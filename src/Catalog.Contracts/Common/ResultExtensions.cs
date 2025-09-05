using Catalog.Domain.Common;

namespace Catalog.Contracts.Common;

public static class ResultExtensions
{
    public static ApiResponse<T> ToApiResponse<T>(this Result<T> result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return ApiResponse<T>.SuccessResult(result.Value!, successMessage);
        }

        return ApiResponse<T>.ErrorResult(result.Error);
    }

    public static ApiResponse<T> ToApiResponse<T>(this Result<T> result, T data, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return ApiResponse<T>.SuccessResult(data, successMessage);
        }
        
        return ApiResponse<T>.ErrorResult(result.Error);
    }

    public static ApiResponse<T> ToApiResponse<T>(this Result result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return ApiResponse<T>.SuccessResult(successMessage);
        }
        
        return ApiResponse<T>.ErrorResult(result.Error);
    }
}