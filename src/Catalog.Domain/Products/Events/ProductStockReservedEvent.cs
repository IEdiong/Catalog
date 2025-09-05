using Catalog.Domain.Common;

namespace Catalog.Domain.Products.Events;

public class ProductStockReservedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public int QuantityReserved { get; }
    public int RemainingStock { get; }

    public ProductStockReservedEvent(Guid productId, int quantityReserved, int remainingStock)
    {
        ProductId = productId;
        QuantityReserved = quantityReserved;
        RemainingStock = remainingStock;
    }
}