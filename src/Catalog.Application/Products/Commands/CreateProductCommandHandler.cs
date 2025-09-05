using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using Catalog.Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Product>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Product>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = new Product(
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity);

            await _unitOfWork.Products.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

            return Result.Success(product);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Failed to create product: {Error}", ex.Message);
            return Result.Failure<Product>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while creating product");
            return Result.Failure<Product>("An unexpected error occurred while creating the product");
        }
    }
}