using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Catalog.Contracts.Common;

namespace Catalog.Api.Filters;

/// <summary>
/// Schema filter to customize ApiResponse documentation in Swagger
/// </summary>
public class ApiResponseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Only apply to ApiResponse<T> but not PaginatedApiResponse<T>
        if (context.Type.IsGenericType && 
            context.Type.GetGenericTypeDefinition() == typeof(ApiResponse<>) &&
            !typeof(PaginatedApiResponse<>).IsAssignableFrom(context.Type.GetGenericTypeDefinition()))
        {
            // Remove error and pagination properties from the schema
            schema.Properties?.Remove("error");
            schema.Properties?.Remove("pagination");
            
            // Also remove them from required properties if they exist
            schema.Required?.Remove("error");
            schema.Required?.Remove("pagination");
        }
        
        // Only apply to PaginatedApiResponse<T>
        if (context.Type.IsGenericType &&
            context.Type.GetGenericTypeDefinition() == typeof(PaginatedApiResponse<>))
        {
            // Remove error property from the schema
            schema.Properties?.Remove("error");
            
            // Also remove it from required properties if it exists
            schema.Required?.Remove("error");
        }
    }
}