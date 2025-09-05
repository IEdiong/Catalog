using Catalog.Domain.Common;

namespace Catalog.Domain.Products.Events;

public class ProductActivatedEvent : DomainEvent
{
    public Guid ProductId { get; }

    public ProductActivatedEvent(Guid productId)
    {
        ProductId = productId;
    }
}