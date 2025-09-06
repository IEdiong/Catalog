using Catalog.Domain.Common;

namespace Catalog.Domain.Products.Events;

public class ProductSoftDeletedEvent : DomainEvent
{
    public Guid ProductId { get; }

    public ProductSoftDeletedEvent(Guid productId)
    {
        ProductId = productId;
    }
}