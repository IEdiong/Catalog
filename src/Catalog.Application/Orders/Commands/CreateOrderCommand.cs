using Catalog.Domain.Common;
using MediatR;

namespace Catalog.Application.Orders.Commands;

public record OrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderCommand(
    string CustomerName,
    string CustomerEmail,
    List<OrderItemRequest> Items) : IRequest<Result<Guid>>;