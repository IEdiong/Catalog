using Catalog.Domain.Common;

namespace Catalog.Domain.Products.Events;

public class ProductUpdatedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public string Name { get; }
    public string Description { get; }
    public decimal Price { get; }

    public ProductUpdatedEvent(Guid productId, string name, string description, decimal price)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Price = price;
    }
}