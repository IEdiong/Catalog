using Catalog.Domain.Common;

namespace Catalog.Domain.Products.Events;

public class ProductDeactivatedEvent : DomainEvent
{
    public Guid ProductId { get; }

    public ProductDeactivatedEvent(Guid productId)
    {
        ProductId = productId;
    }
}