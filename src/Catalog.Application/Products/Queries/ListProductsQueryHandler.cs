using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using Catalog.Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.Queries;

public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, Result<List<Product>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ListProductsQueryHandler> _logger;

    public ListProductsQueryHandler(ILogger<ListProductsQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result<List<Product>>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
            
            var result = products.ToList();
            _logger.LogInformation("Retrieved {ProductCount} products", result.ToList().Count);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while listing products");
            return Result.Failure<List<Product>>("An unexpected error occurred while listing products");
        }
    }
}