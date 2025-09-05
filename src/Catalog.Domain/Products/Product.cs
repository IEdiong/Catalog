using Catalog.Domain.Common;
using Catalog.Domain.Products.Events;

namespace Catalog.Domain.Products;

public class Product : AggregateRoot
{
    public const int MaxNameLength = 255;
    public const int MaxDescriptionLength = 1000;
    public const decimal MaxPrice = 1_000_000;
    public const int MaxStockQuantity = 100_000;

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public uint Version { get; private set; }

    public Product(string name, string description, decimal price, int stockQuantity, Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetStockQuantity(stockQuantity);
        CreatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price, StockQuantity));
    }
    
    private Product() { }

    public Result UpdateDetails(string name, string description, decimal price)
    {
        try
        {
            SetName(name);
            SetDescription(description);
            SetPrice(price);
            UpdatedAt = DateTime.UtcNow;
            Version++;
            
            AddDomainEvent(new ProductUpdatedEvent(Id, Name, Description, Price));
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result ReserveStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure("Quantity must be greater than zero");
            
        if (!IsActive)
            return Result.Failure("Product is not active");
            
        if (StockQuantity < quantity)
            return Result.Failure($"Insufficient stock. Available: {StockQuantity}, Requested: {quantity}");

        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
        Version++;
        
        AddDomainEvent(new ProductStockReservedEvent(Id, quantity, StockQuantity));
        return Result.Success();
    }

    public Result AddStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure("Quantity must be greater than zero");
            
        if (StockQuantity + quantity > MaxStockQuantity)
            return Result.Failure($"Stock quantity cannot exceed {MaxStockQuantity}");

        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
        Version++;
        
        AddDomainEvent(new ProductStockAddedEvent(Id, quantity, StockQuantity));
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Failure("Product is already inactive");
            
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        Version++;
        
        AddDomainEvent(new ProductDeactivatedEvent(Id));
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
            return Result.Failure("Product is already active");
            
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        Version++;
        
        AddDomainEvent(new ProductActivatedEvent(Id));
        return Result.Success();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));
            
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Product name cannot exceed {MaxNameLength} characters", nameof(name));
            
        Name = name.Trim();
    }

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description cannot be empty", nameof(description));
            
        if (description.Length > MaxDescriptionLength)
            throw new ArgumentException($"Product description cannot exceed {MaxDescriptionLength} characters", nameof(description));
            
        Description = description.Trim();
    }

    private void SetPrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Product price must be greater than zero", nameof(price));
            
        if (price > MaxPrice)
            throw new ArgumentException($"Product price cannot exceed {MaxPrice}", nameof(price));
            
        Price = Math.Round(price, 2);
    }

    private void SetStockQuantity(int stockQuantity)
    {
        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));
            
        if (stockQuantity > MaxStockQuantity)
            throw new ArgumentException($"Stock quantity cannot exceed {MaxStockQuantity}", nameof(stockQuantity));
            
        StockQuantity = stockQuantity;
    }
}