using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.Commands;

public class RestoreProductCommandHandler : IRequestHandler<RestoreProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreProductCommandHandler> _logger;
    
    public RestoreProductCommandHandler(IUnitOfWork unitOfWork, ILogger<RestoreProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result> Handle(RestoreProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning($"Product with id {request.Id} was not found");
                return Result.Failure("Product not found");
            }
            
            var result = product.Restore();
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to restore product with ID {ProductId}: {Error}", request.Id, result.Error);
                return result;
            }
            
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Product with ID {ProductId} restored successfully", request.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while restoring product {ProductId}", request.Id);
            return Result.Failure("An unexpected error occurred while restoring the product");
        }
    }
}