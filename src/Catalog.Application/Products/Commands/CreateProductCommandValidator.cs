using Catalog.Domain.Products;
using FluentValidation;

namespace Catalog.Application.Products.Commands;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(Product.MaxNameLength).WithMessage($"Product name cannot exceed {Product.MaxNameLength} characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required")
            .MaximumLength(Product.MaxDescriptionLength).WithMessage($"Product description cannot exceed {Product.MaxDescriptionLength} characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than zero")
            .LessThanOrEqualTo(Product.MaxPrice).WithMessage($"Product price cannot exceed {Product.MaxPrice}");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative")
            .LessThanOrEqualTo(Product.MaxStockQuantity).WithMessage($"Stock quantity cannot exceed {Product.MaxStockQuantity}");
    }
}