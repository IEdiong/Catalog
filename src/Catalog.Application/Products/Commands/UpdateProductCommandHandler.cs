using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using Catalog.Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<Product>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Product>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                return Result.Failure<Product>("Product not found");
            }

            var result = product.UpdateDetails(request.Name, request.Description, request.Price);
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to update product {ProductId}: {Error}", request.Id, result.Error);
                return Result.Failure<Product>(result.Error);
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product {ProductId} updated successfully", request.Id);
            return Result.Success(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while updating product {ProductId}", request.Id);
            return Result.Failure<Product>("An unexpected error occurred while updating the product");
        }
    }
}