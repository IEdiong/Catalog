namespace Catalog.Contracts.Common;

/// <summary>
/// Standard API response wrapper for all endpoints
/// </summary>
/// <typeparam name="T">The type of data being returned</typeparam>
public record ApiResponse<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public ApiError? Error { get; init; }
    public PaginationMeta? Pagination { get; init; }


    // Success responses
    public static ApiResponse<T> SuccessResult(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static ApiResponse<T> SuccessResult(string? message = null) => new()
    {
        Success = true,
        Message = message
    };

    // Error responses
    public static ApiResponse<T> ErrorResult(string error, string? message = null, int? statusCode = null) => new()
    {
        Success = false,
        Error = new ApiError(error, statusCode),
        Message = message
    };

    public static ApiResponse<T> ErrorResult(ApiError error, string? message = null) => new()
    {
        Success = false,
        Error = error,
        Message = message
    };
};

/// <summary>
/// Standard API response wrapper for paginated data
/// </summary>
/// <typeparam name="T">The type of items in the collection</typeparam>
public record PaginatedApiResponse<T> : ApiResponse<IEnumerable<T>>
{
    public PaginatedApiResponse(IEnumerable<T> data, PaginationMeta pagination, string? message = null)
    {
        Success = true;
        Data = data;
        Pagination = pagination;
        Message = message;
    }

    public static PaginatedApiResponse<T> SuccessResult(IEnumerable<T> data, PaginationMeta pagination,
        string? message = null) => new(data, pagination, message);
}

/// <summary>
/// Error information for API responses
/// </summary>
public record ApiError
{
    public string Message { get; init; }
    public int? StatusCode { get; init; }
    public string? Code { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    public ApiError(string message, int? statusCode = null, string? code = null,
        Dictionary<string, string[]>? validationErrors = null)
    {
        Message = message;
        StatusCode = statusCode;
        Code = code;
        ValidationErrors = validationErrors;
    }
}

/// <summary>
/// Pagination metadata for collection responses
/// </summary>
public record PaginationMeta
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasNext { get; init; }
    public bool HasPrevious { get; init; }

    public PaginationMeta(int page, int pageSize, int totalCount)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasNext = Page < TotalPages;
        HasPrevious = Page > 1;
    }
}