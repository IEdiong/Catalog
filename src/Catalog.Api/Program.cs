using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Catalog.Api.Configurations;
using Catalog.Api.Filters;
using Catalog.Api.Middleware;
using Catalog.Application;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/catalog-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Add API Versioning
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = false;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new QueryStringApiVersionReader("version"),
            new HeaderApiVersionReader("X-Version"),
            new MediaTypeApiVersionReader("ver"));
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfiguration>();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<ApiResponseSchemaFilter>();
});

// Add application layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ??
               "Host=localhost;Database=CatalogDb;Username=postgres;Password=postgres");

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Now we can safely access services since the app is built
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        
        foreach (var description in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"Catalog API v{description.ApiVersion}");
        }
    });
}

app.UseSerilogRequestLogging();

app.UseCors("DefaultPolicy");
app.UseHttpsRedirection();

app.MapControllers();

// Health checks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DbInitializer.InitializeAsync(context, logger);
}

try
{
    Log.Information("Starting Catalog API");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Catalog API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}