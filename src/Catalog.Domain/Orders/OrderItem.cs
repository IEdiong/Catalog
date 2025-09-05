using Catalog.Domain.Common;

namespace Catalog.Domain.Orders;

public class OrderItem : Entity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public decimal LineTotal => Price * Quantity;

    public OrderItem(Guid productId, string productName, decimal price, int quantity, Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));
            
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));
            
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));
            
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            
        ProductId = productId;
        ProductName = productName.Trim();
        Price = Math.Round(price, 2);
        Quantity = quantity;
    }
    
    private OrderItem() { }
}