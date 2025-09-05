using Catalog.Application.Products.Commands;
using Catalog.Contracts.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Product Creation Failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        var response = new CreateProductResponse(result.Value!);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Value }, response);
    }

    /// <summary>
    /// Get a product by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        // This would need a GetProductQuery - simplified for now
        return Ok(new ProductResponse(id, "Sample Product", "Sample Description", 99.99m, 10, true));
    }

    /// <summary>
    /// Update a product
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        var command = new UpdateProductCommand(id, request.Name, request.Description, request.Price);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            var statusCode = result.Error.Contains("not found")
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status400BadRequest;

            return Problem(
                title: "Product Update Failed",
                detail: result.Error,
                statusCode: statusCode);
        }

        return NoContent();
    }

    /// <summary>
    /// Get all active products
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search, 
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // This would need a GetProductsQuery - simplified for now
        var products = new List<ProductResponse>
        {
            new(Guid.NewGuid(), "Sample Product 1", "Sample Description 1", 99.99m, 10, true),
            new(Guid.NewGuid(), "Sample Product 2", "Sample Description 2", 149.99m, 5, true)
        };

        return Ok(products);
    }
}