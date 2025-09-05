using Catalog.Domain.Common;
using Catalog.Domain.Products;
using MediatR;

namespace Catalog.Application.Products.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity) : IRequest<Result<Product>>;