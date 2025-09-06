using FluentValidation;

namespace Catalog.Application.Orders.Queries.ListOrdersByEmail;

public class ListOrdersByEmailQueryValidator : AbstractValidator<ListOrdersByEmailQuery>
{
    public ListOrdersByEmailQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Customer email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");
    }
}