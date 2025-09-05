using Catalog.Domain.Common;
using Catalog.Domain.Orders.Events;

namespace Catalog.Domain.Orders;

public class Order : AggregateRoot
{
    public const int MaxCustomerNameLength = 255;
    public const int MaxCustomerEmailLength = 255;
    
    private readonly List<OrderItem> _items = new();
    
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public uint Version { get; private set; }

    public Order(string customerName, string customerEmail, List<OrderItem> items, Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        SetCustomerName(customerName);
        SetCustomerEmail(customerEmail);
        
        if (items == null || !items.Any())
            throw new ArgumentException("Order must have at least one item", nameof(items));
            
        _items.AddRange(items);
        CalculateTotalAmount();
        
        Status = OrderStatus.Pending;
        OrderDate = DateTime.UtcNow;
        
        AddDomainEvent(new OrderCreatedEvent(Id, CustomerName, CustomerEmail, TotalAmount, Items.Count));
    }
    
    private Order() { }

    public Result Complete()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure($"Cannot complete order with status {Status}");
            
        Status = OrderStatus.Completed;
        CompletedDate = DateTime.UtcNow;
        Version++;
        
        AddDomainEvent(new OrderCompletedEvent(Id, TotalAmount));
        return Result.Success();
    }

    public Result Cancel(string reason)
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure($"Cannot cancel order with status {Status}");
            
        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure("Cancellation reason is required");
            
        Status = OrderStatus.Cancelled;
        Version++;
        
        AddDomainEvent(new OrderCancelledEvent(Id, reason));
        return Result.Success();
    }

    private void SetCustomerName(string customerName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name cannot be empty", nameof(customerName));
            
        if (customerName.Length > MaxCustomerNameLength)
            throw new ArgumentException($"Customer name cannot exceed {MaxCustomerNameLength} characters", nameof(customerName));
            
        CustomerName = customerName.Trim();
    }

    private void SetCustomerEmail(string customerEmail)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email cannot be empty", nameof(customerEmail));
            
        if (customerEmail.Length > MaxCustomerEmailLength)
            throw new ArgumentException($"Customer email cannot exceed {MaxCustomerEmailLength} characters", nameof(customerEmail));
            
        if (!IsValidEmail(customerEmail))
            throw new ArgumentException("Invalid email format", nameof(customerEmail));
            
        CustomerEmail = customerEmail.Trim().ToLowerInvariant();
    }

    private void CalculateTotalAmount()
    {
        TotalAmount = _items.Sum(item => item.Price * item.Quantity);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}