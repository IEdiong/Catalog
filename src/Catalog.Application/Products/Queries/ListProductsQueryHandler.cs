using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using Catalog.Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.Queries;

public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, Result<ListProductsResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ListProductsQueryHandler> _logger;

    public ListProductsQueryHandler(ILogger<ListProductsQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result<ListProductsResult>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate pagination parameters
            if (request.Page < 1)
                return Result.Failure<ListProductsResult>("Page number must be at least 1");
            if (request.PageSize < 1 || request.PageSize > 100)
                return Result.Failure<ListProductsResult>("Page size must be between 1 and 100");
            _logger.LogInformation("Listing products with Search: {Search}, Page: {Page}, PageSize: {PageSize}",
                request.Search, request.Page, request.PageSize);

            IEnumerable<Product> products;
            int totalCount;

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                // Use search functionality
                var skip = (request.Page - 1) * request.PageSize;
                products = await _unitOfWork.Products.SearchProductsAsync(request.Search, skip, request.PageSize,
                    cancellationToken);

                // For search, we need to get the total count separately
                // This could be optimized with a different repository method if needed
                // This is a limitation - ideally the repository should return both results and total count
                var allSearchResults =
                    await _unitOfWork.Products.SearchProductsAsync(request.Search, 0, int.MaxValue, cancellationToken);
                totalCount = allSearchResults.Count();
            }
            else
            {
                // Get all active products with pagination
                var skip = (request.Page - 1) * request.PageSize;
                products = await _unitOfWork.Products.GetActiveProductsAsync(cancellationToken);
                
                // Apply pagination in memory (not ideal for large datasets)
                totalCount = products.Count();
                products = products.Skip(skip).Take(request.PageSize);
            }
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
            var hasNext = request.Page < totalPages;
            var hasPrevious = request.Page > 1;

            var result = new ListProductsResult(
                products,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages,
                hasNext,
                hasPrevious);

            _logger.LogInformation("Retrieved {ProductCount} products (page {Page} of {TotalPages})", 
                products.Count(), request.Page, totalPages);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while listing products");
            return Result.Failure<ListProductsResult>("An unexpected error occurred while listing products");
        }
    }
}