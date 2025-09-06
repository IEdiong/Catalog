using Catalog.Domain.Common;
using Catalog.Domain.Orders;
using MediatR;

namespace Catalog.Application.Orders.Queries.GetOrder;

public record GetOrderQuery(Guid Id) : IRequest<Result<Order>>;