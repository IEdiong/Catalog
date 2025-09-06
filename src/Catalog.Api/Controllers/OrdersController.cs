using Catalog.Api.Common;
using Catalog.Api.Mappings;
using Catalog.Application.Orders.Commands;
using Catalog.Application.Orders.Queries.GetOrder;
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
    /// Get orders by customer email
    /// </summary>
    [HttpGet("customer/{email}")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrdersByCustomer(string email)
    {
        // This would need a GetOrdersByCustomerQuery - simplified for now
        var orders = new List<OrderResponse>();
        return Ok(orders);
    }
}