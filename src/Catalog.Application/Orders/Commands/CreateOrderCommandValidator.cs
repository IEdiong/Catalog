using Catalog.Domain.Orders;
using FluentValidation;

namespace Catalog.Application.Orders.Commands;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(Order.MaxCustomerNameLength).WithMessage($"Customer name cannot exceed {Order.MaxCustomerNameLength} characters");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(Order.MaxCustomerEmailLength).WithMessage($"Customer email cannot exceed {Order.MaxCustomerEmailLength} characters");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must have at least one item")
            .Must(HaveUniqueProducts).WithMessage("Duplicate products in order items are not allowed");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemValidator());
    }

    private static bool HaveUniqueProducts(List<OrderItemRequest> items)
    {
        var productIds = items.Select(i => i.ProductId).ToList();
        return productIds.Count == productIds.Distinct().Count();
    }
}

public class OrderItemValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero")
            .LessThanOrEqualTo(1000).WithMessage("Quantity cannot exceed 1000 items per product");
    }
}