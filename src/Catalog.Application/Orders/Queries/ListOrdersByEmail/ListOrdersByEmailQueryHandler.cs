using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Orders.Queries.ListOrdersByEmail;

public class ListOrdersByEmailQueryHandler : IRequestHandler<ListOrdersByEmailQuery, Result<ListOrdersResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ListOrdersByEmailQueryHandler> _logger;

    public ListOrdersByEmailQueryHandler(ILogger<ListOrdersByEmailQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ListOrdersResult>> Handle(ListOrdersByEmailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate pagination parameters
            if (request.Page < 1)
                return Result.Failure<ListOrdersResult>("Page number must be at least 1");
            
            if (request.PageSize < 1 || request.PageSize > 100)
                return Result.Failure<ListOrdersResult>("Page size must be between 1 and 100");

            // Validate email format (basic validation)
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure<ListOrdersResult>("Customer email is required");

            _logger.LogInformation("Listing orders for customer email: {Email}, Page: {Page}, PageSize: {PageSize}",
                request.Email, request.Page, request.PageSize);

            // Get all orders for the customer email
            var allOrders = await _unitOfWork.Orders.GetOrdersByCustomerEmailAsync(request.Email, cancellationToken);
            
            // Apply search filter if provided
            IEnumerable<Domain.Orders.Order> filteredOrders = allOrders;
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                filteredOrders = allOrders.Where(o => 
                    o.CustomerName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                    o.CustomerEmail.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                    o.Status.ToString().Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            // Calculate pagination
            var totalCount = filteredOrders.Count();
            var skip = (request.Page - 1) * request.PageSize;
            var paginatedOrders = filteredOrders
                .OrderByDescending(o => o.OrderDate) // Most recent orders first
                .Skip(skip)
                .Take(request.PageSize);

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
            var hasNext = request.Page < totalPages;
            var hasPrevious = request.Page > 1;

            var result = new ListOrdersResult(
                paginatedOrders,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages,
                hasNext,
                hasPrevious);

            _logger.LogInformation("Retrieved {OrderCount} orders for customer {Email} (page {Page} of {TotalPages})", 
                paginatedOrders.Count(), request.Email, request.Page, totalPages);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order with customer email {CustomerEmail}", request.Email);
            return Result.Failure<ListOrdersResult>("An error occurred while retrieving the order");
        }
    }
}