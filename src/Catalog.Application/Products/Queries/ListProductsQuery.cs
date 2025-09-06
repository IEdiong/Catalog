using Catalog.Domain.Common;
using Catalog.Domain.Products;
using MediatR;

namespace Catalog.Application.Products.Queries;

public record ListProductsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 10)
    : IRequest<Result<ListProductsResult>>;