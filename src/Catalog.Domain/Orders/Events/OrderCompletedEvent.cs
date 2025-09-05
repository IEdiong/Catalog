using Catalog.Domain.Common;

namespace Catalog.Domain.Orders.Events;

public class OrderCompletedEvent : DomainEvent
{
    public Guid OrderId { get; }
    public decimal TotalAmount { get; }

    public OrderCompletedEvent(Guid orderId, decimal totalAmount)
    {
        OrderId = orderId;
        TotalAmount = totalAmount;
    }
}