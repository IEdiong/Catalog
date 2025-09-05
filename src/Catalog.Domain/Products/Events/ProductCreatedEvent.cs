using Catalog.Domain.Common;

namespace Catalog.Domain.Products.Events;

public class ProductCreatedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public string Name { get; }
    public decimal Price { get; }
    public int InitialStock { get; }

    public ProductCreatedEvent(Guid productId, string name, decimal price, int initialStock)
    {
        ProductId = productId;
        Name = name;
        Price = price;
        InitialStock = initialStock;
    }
}