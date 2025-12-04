using FluentValidation;

namespace SampleApp.Models
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {

            RuleFor(x => x.Id)
                .NotNull()
                .WithMessage("Product ID is required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Product name is required")
                .MaximumLength(200)
                .WithMessage("Product name cannot exceed 200 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(100000)
                .WithMessage("Price cannot exceed 100,000");

            RuleFor(x => x.Sku)
                .NotEmpty()
                .WithMessage("SKU is required")
                .Matches("^[A-Z]{3}-\\d{4}$")
                .WithMessage("SKU must follow format: XXX-9999");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters");
        }
    }
}
