using FluentValidation;

namespace Catalog.Application.Products.Commands;

public class HardDeleteProductCommandValidator : AbstractValidator<HardDeleteProductCommand>
{
    public HardDeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}