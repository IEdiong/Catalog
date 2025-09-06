using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using Catalog.Domain.Orders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Orders.Queries.GetOrder;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Result<Order>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetOrderQueryHandler> _logger;

    public GetOrderQueryHandler(IUnitOfWork unitOfWork, ILogger<GetOrderQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(request.Id, cancellationToken);
            if (order == null)
            {
                _logger.LogError($"Order with ID {request.Id} was not found");
                return Result.Failure<Order>("An error occurred while retrieving the order");
            }
            
            _logger.LogInformation($"Order with ID {request.Id} was found");
            return Result.Success(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order with ID {OrderId}", request.Id);
            return Result.Failure<Order>("An error occurred while retrieving the order");
        }
    }
}