using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using Catalog.Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.Queries;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<Product>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetProductQueryHandler> _logger;

    public GetProductQueryHandler(IUnitOfWork unitOfWork, ILogger<GetProductQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Product>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
                return Result.Failure<Product>("Product not found");
            }
            
            _logger.LogInformation("Getting product with ID {ProductId}", request.Id);
            return Result.Success(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", request.Id);
            return Result.Failure<Product>("An error occurred while retrieving the product");
        }
    }
}