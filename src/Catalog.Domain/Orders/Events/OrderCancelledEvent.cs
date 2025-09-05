using Catalog.Domain.Common;

namespace Catalog.Domain.Orders.Events;

public class OrderCancelledEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string Reason { get; }

    public OrderCancelledEvent(Guid orderId, string reason)
    {
        OrderId = orderId;
        Reason = reason;
    }
}