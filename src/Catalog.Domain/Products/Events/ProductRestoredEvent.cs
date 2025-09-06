using Catalog.Domain.Common;

namespace Catalog.Domain.Products.Events;

public class ProductRestoredEvent : DomainEvent
{
    public Guid ProductId { get; }
    public ProductRestoredEvent(Guid productId)
    {
        ProductId = productId;
    }
}