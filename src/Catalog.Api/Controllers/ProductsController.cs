using Catalog.Api.Common;
using Catalog.Api.Mappings;
using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Queries;
using Catalog.Contracts.Common;
using Catalog.Contracts.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : BaseApiController
{
    private readonly ISender _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ISender mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateProductResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error, "Product creation failed");
        }

        var product = result.Value!;
        var response = new CreateProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.CreatedAt);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, response, "Product created successfully");
    }

    /// <summary>
    /// Update a product
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>),StatusCodes.Status200OK)]
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
        
        var product = result.Value!.ToProductResponse();
        return Ok(product, "Product updated successfully");
    }

    /// <summary>
    /// Get a product by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id)
    {
        var query = new GetProductQuery(id);
        var result = await _mediator.Send(query);
        
        if (result.IsFailure)
        {
            return NotFound(result.Error, "Product not found");
        }
        
        var product = result.Value!.ToProductResponse();
        
        return Ok(product);
    }

    /// <summary>
    /// Get all active products with search and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedApiResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Validate pagination parameters
        if (page < 1)
            return BadRequest("Page must be greater than 0");

        if (pageSize < 1 || pageSize > 100)
            return BadRequest("Page size must be between 1 and 100");

        var query = new ListProductsQuery(search, page, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error, "Failed to retrieve products");
        }

        // Map domain entities to DTOs in the API layer
        var products = result.Value!.Products.ToProductResponses();
        var pagination = new PaginationMeta(
            result.Value.Page,
            result.Value.PageSize,
            result.Value.TotalCount);

        return Ok(products, pagination, "Products retrieved successfully");
    }

    /// <summary>
    /// Soft delete a product (mark as inactive)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SoftDeleteProduct(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new SoftDeleteProductCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
                return NotFound(result.Error, "Product not found");

            return BadRequest(result.Error, "Failed to delete product");
        }

        return Ok("Product deleted successfully");
    }

    /// <summary>
    /// Permanently delete a product from the database
    /// </summary>
    [HttpDelete("{id:guid}/permanent")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HardDeleteProduct(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new HardDeleteProductCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
                return NotFound(result.Error, "Product not found");

            return BadRequest(result.Error, "Failed to permanently delete product");
        }

        return Ok("Product permanently deleted");
    }

    /// <summary>
    /// Restore a soft-deleted product
    /// </summary>
    [HttpPatch("{id:guid}/restore")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RestoreProduct(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new RestoreProductCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
                return NotFound(result.Error, "Product not found");

            return BadRequest(result.Error, "Failed to restore product");
        }

        return Ok("Product restored successfully");
    }
}