using Catalog.Contracts.Orders;
using Catalog.Domain.Orders;

namespace Catalog.Api.Mappings;

public static class OrderMappings
{
    public static OrderResponse ToOrderResponse(this Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CustomerName,
            order.CustomerEmail,
            order.Status.ToString(),
            order.TotalAmount,
            order.OrderDate,
            order.CompletedDate,
            order.Items.Select(i => new OrderItemResponse(
                    i.ProductId,
                    i.ProductName,
                    i.Price,
                    i.Quantity,
                    i.LineTotal))
                .ToList());
    }

    public static IEnumerable<OrderResponse> TOrderResponses(this IEnumerable<Order> orders)
    {
        return orders.Select(ToOrderResponse);
    }
}