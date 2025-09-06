using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.Commands;

public class SoftDeleteProductCommandHandler : IRequestHandler<SoftDeleteProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SoftDeleteProductCommandHandler> _logger;
    
    public SoftDeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<SoftDeleteProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result> Handle(SoftDeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for soft deletion", request.ProductId);
                return Result.Failure("Product not found.");
            }

            var result = product.SoftDelete();
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to soft delete product with ID {ProductId}: {Error}", request.ProductId, result.Error);
                return result;
            }
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product with ID {ProductId} soft deleted successfully", request.ProductId);
            return Result.Success();
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error occurred while soft deleting product with ID {ProductId}", request.ProductId);
            return Result.Failure("A database error occurred while deleting the product.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while soft deleting product with ID {ProductId}", request.ProductId);
            return Result.Failure("An unexpected error occurred while deleting the product.");
        }
    }
}