using Catalog.Domain.Common;

namespace Catalog.Domain.Products.Events;

public class ProductStockAddedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public int QuantityAdded { get; }
    public int NewStockLevel { get; }

    public ProductStockAddedEvent(Guid productId, int quantityAdded, int newStockLevel)
    {
        ProductId = productId;
        QuantityAdded = quantityAdded;
        NewStockLevel = newStockLevel;
    }
}