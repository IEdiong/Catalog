using Catalog.Api.Common;
using Catalog.Api.Mappings;
using Catalog.Application.Orders.Commands;
using Catalog.Application.Orders.Queries.GetOrder;
using Catalog.Application.Orders.Queries.ListOrdersByEmail;
using Catalog.Contracts.Common;
using Catalog.Contracts.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController: BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Place a new order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var orderItems = request.Items.Select(i => 
            new OrderItemRequest(i.ProductId, i.Quantity)).ToList();

        var command = new CreateOrderCommand(
            request.CustomerName,
            request.CustomerEmail,
            orderItems);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            // Determine appropriate status code based on error type
            var statusCode = StatusCodes.Status400BadRequest;
            if (result.Error.Contains("not found"))
                statusCode = StatusCodes.Status404NotFound;
            else if (result.Error.Contains("stock") || result.Error.Contains("changed"))
                statusCode = StatusCodes.Status409Conflict;

            return Problem(
                title: "Order Creation Failed",
                detail: result.Error,
                statusCode: statusCode);
        }

        var response = new CreateOrderResponse(result.Value!);
        return CreatedAtAction(nameof(GetOrder), new { id = result.Value }, response);
    }

    /// <summary>
    /// Get an order by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder([FromRoute] Guid id)
    {
        var query = new GetOrderQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(result.Error, "Order not found");
        }
        
        var response = result.Value!.ToOrderResponse();
        return Ok(response);
    }

    /// <summary>
    /// Get orders by customer email with pagination and search
    /// </summary>
    [HttpGet("customer/{email}")]
    [ProducesResponseType(typeof(PaginatedApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrdersByCustomer(
        [FromRoute] string email,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Validate pagination parameters
        if (page < 1)
            return BadRequest("Page number must be at least 1");
        
        if (pageSize < 1 || pageSize > 100)
            return BadRequest("Page size must be between 1 and 100");

        // Validate email format (basic validation)
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Customer email is required");

        var query = new ListOrdersByEmailQuery(email, search, page, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error, "Failed to retrieve orders");
        }

        // Map domain entities to DTOs
        var orders = result.Value!.Orders.ToOrderResponses();
        var pagination = new PaginationMeta(
            result.Value.Page,
            result.Value.PageSize,
            result.Value.TotalCount);

        return Ok(orders, pagination, "Orders retrieved successfully");
    }
}