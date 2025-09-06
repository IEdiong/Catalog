using Catalog.Domain.Products;

namespace Catalog.Application.Products.Queries;

public record ListProductsResult(
    IEnumerable<Product> Products,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);