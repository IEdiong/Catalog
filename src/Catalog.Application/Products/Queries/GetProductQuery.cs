using Catalog.Domain.Common;
using Catalog.Domain.Products;
using MediatR;

namespace Catalog.Application.Products.Queries;

public record GetProductQuery(Guid Id) : IRequest<Result<Product>>;