using Catalog.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Common;

/// <summary>
/// Base controller
/// </summary>
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Returns a successful response with data and optional message
    /// </summary>
    /// <param name="data">The data to be returned</param>
    /// <param name="message">The success message to be returned</param>
    /// <typeparam name="T">The type of data returned</typeparam>
    protected IActionResult Ok<T>(T data, string? message = null)
        => base.Ok(ApiResponse<T>.SuccessResult(data, message));

    /// <summary>
    /// Returns a successful response with paginated data and optional message
    /// </summary>
    protected IActionResult Ok<T>(IEnumerable<T> data, PaginationMeta pagination, string? message = null)
        => base.Ok(new PaginatedApiResponse<T>(data, pagination, message));

    /// <summary>
    /// Returns a successful response with only a message
    /// </summary>
    protected IActionResult Ok(string message) => base.Ok(ApiResponse<object>.SuccessResult(message));


    /// <summary>
    /// Returns a CreatedAtAction response with data and optional message
    /// </summary>
    protected IActionResult CreatedAtAction<T>(string? actionName, object? routeValues, T data, string? message = null)
        => base.CreatedAtAction(actionName, routeValues, ApiResponse<T>.SuccessResult(data, message));

    /// <summary>
    /// Returns a Created response with data and optional message
    /// </summary>
    protected IActionResult Created<T>(string? uri, T data, string? message = null)
        => base.Created(uri, ApiResponse<T>.SuccessResult(data, message));

    /// <summary>
    /// Returns a bad request response with an error message
    /// </summary>
    protected IActionResult BadRequest(string error, string? message = null, int? statusCode = null) =>
        base.BadRequest(ApiResponse<object>.ErrorResult(error, message, statusCode));

    /// <summary>
    /// Returns a bad request with validation errors
    /// </summary>
    protected IActionResult BadRequest(Dictionary<string, string[]> validationErrors, string? message = null)
    {
        var errors = new ApiError("Validation Failed", 400, "VALIDATION_ERROR", validationErrors);
        return base.BadRequest(ApiResponse<object>.ErrorResult(errors, message));
    }

    /// <summary>
    /// Returns a not found response with an error message
    /// </summary>
    protected IActionResult NotFound(string error = "Resource Not Found", string? message = null) =>
        base.NotFound(ApiResponse<object>.ErrorResult(error, message, 404));

    /// <summary>
    /// Returns a conflict response with an error message
    /// </summary>
    protected IActionResult Conflict(string error, string? message = null) =>
        base.Conflict(ApiResponse<object>.ErrorResult(error, message, 409));

    protected IActionResult InternalServerError(string error = "An internal server error occurred",
        string? message = null) => StatusCode(500, ApiResponse<object>.ErrorResult(error, message, 500));
}