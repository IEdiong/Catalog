using Catalog.Domain.Common;
using Catalog.Domain.Products;
using MediatR;

namespace Catalog.Application.Products.Queries;

public record ListProductsQuery() : IRequest<Result<List<Product>>>;