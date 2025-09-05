using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Catalog.Api.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        object response = new
        {
            Title = "An error occurred",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = "An unexpected error occurred. Please try again later.",
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ValidationException validationEx:
                response = new
                {
                    Title = "Validation failed",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = "One or more validation errors occurred",
                    Errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
                    TraceId = context.TraceIdentifier
                };
                break;

            case ArgumentException argEx:
                response = new
                {
                    Title = "Invalid argument",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = argEx.Message,
                    TraceId = context.TraceIdentifier
                };
                break;

            case UnauthorizedAccessException:
                response = new
                {
                    Title = "Unauthorized",
                    Status = (int)HttpStatusCode.Unauthorized,
                    Detail = "You are not authorized to perform this action",
                    TraceId = context.TraceIdentifier
                };
                break;
        }

        context.Response.ContentType = "application/json";
        
        var statusCode = HttpStatusCode.InternalServerError;
        if (response is { } responseObj)
        {
            var responseType = responseObj.GetType();
            var statusProperty = responseType.GetProperty("Status");
            if (statusProperty != null)
            {
                statusCode = (HttpStatusCode)(int)statusProperty.GetValue(responseObj)!;
            }
        }
        
        context.Response.StatusCode = (int)statusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}