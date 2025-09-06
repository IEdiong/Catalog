using Catalog.Domain.Orders;

namespace Catalog.Application.Orders.Queries.ListOrdersByEmail;

public record ListOrdersResult(
    IEnumerable<Order> Orders,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);