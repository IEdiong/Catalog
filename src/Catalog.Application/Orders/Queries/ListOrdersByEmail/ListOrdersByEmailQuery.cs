using Catalog.Domain.Common;
using Catalog.Domain.Orders;
using MediatR;

namespace Catalog.Application.Orders.Queries.ListOrdersByEmail;

public record ListOrdersByEmailQuery(
    string Email,
    string? Search = null,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<ListOrdersResult>>;