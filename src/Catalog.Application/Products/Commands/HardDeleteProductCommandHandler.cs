using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.Commands;

public class HardDeleteProductCommandHandler : IRequestHandler<HardDeleteProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HardDeleteProductCommandHandler> _logger;

    public HardDeleteProductCommandHandler(ILogger<HardDeleteProductCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(HardDeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for hard deletion", request.ProductId);
                return Result.Failure("Product not found");
            }
            
            // Check if product has any orders (business rule)
            // This would require checking order items - simplified for now
            // In a real scenario, you might want to prevent hard delete if there are orders
            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Product with ID {ProductId} hard deleted successfully", request.ProductId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while permanently deleting product {ProductId}", request.ProductId);
            return Result.Failure("An unexpected error occurred while deleting the product");
        }
    }
}