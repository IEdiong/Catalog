using FluentValidation;

namespace Catalog.Application.Products.Commands;

public class SoftDeleteProductCommandValidator : AbstractValidator<SoftDeleteProductCommand>
{
    public SoftDeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}