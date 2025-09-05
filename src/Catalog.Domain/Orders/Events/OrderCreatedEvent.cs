using Catalog.Domain.Common;

namespace Catalog.Domain.Orders.Events;

public class OrderCreatedEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string CustomerName { get; }
    public string CustomerEmail { get; }
    public decimal TotalAmount { get; }
    public int ItemCount { get; }

    public OrderCreatedEvent(Guid orderId, string customerName, string customerEmail, decimal totalAmount, int itemCount)
    {
        OrderId = orderId;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
        ItemCount = itemCount;
    }
}