using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using Catalog.Domain.Orders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Orders.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Get all required products with row-level locks to prevent concurrent stock modifications
            var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _unitOfWork.Products.GetProductsByIdsWithLockAsync(productIds, cancellationToken);
            var productDict = products.ToDictionary(p => p.Id);

            // Validate all products exist and are active
            var orderItems = new List<OrderItem>();
            foreach (var item in request.Items)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<Guid>($"Product with ID {item.ProductId} not found");
                }

                if (!product.IsActive)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<Guid>($"Product '{product.Name}' is not active");
                }

                // Create order item with current product information
                var orderItem = new OrderItem(product.Id, product.Name, product.Price, item.Quantity);
                orderItems.Add(orderItem);
            }

            // Reserve stock for all products (this is where concurrency control happens)
            foreach (var item in request.Items)
            {
                var product = productDict[item.ProductId];
                var reserveResult = product.ReserveStock(item.Quantity);
                
                if (reserveResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<Guid>($"Failed to reserve stock for '{product.Name}': {reserveResult.Error}");
                }

                _unitOfWork.Products.Update(product);
            }

            // Create the order
            var order = new Order(request.CustomerName, request.CustomerEmail, orderItems);
            
            await _unitOfWork.Orders.AddAsync(order, cancellationToken);
            
            // Save all changes within the transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerEmail} with {ItemCount} items, total: {TotalAmount:C}", 
                order.Id, order.CustomerEmail, order.Items.Count, order.TotalAmount);

            return Result.Success(order.Id);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("concurrency"))
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning("Concurrency conflict occurred while creating order: {Error}", ex.Message);
            return Result.Failure<Guid>("Stock levels have changed. Please try again.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Unexpected error occurred while creating order");
            return Result.Failure<Guid>("An unexpected error occurred while creating the order");
        }
    }
}