using FluentValidation;

namespace SampleApp.Models
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {

            RuleFor(x => x.Street)
                .NotEmpty()
                .WithMessage("Street address is required")
                .Length(5, 200)
                .WithMessage("Street address must be between 5 and 200 characters");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required")
                .MaximumLength(100)
                .WithMessage("City cannot exceed 100 characters");

            RuleFor(x => x.State)
                .NotEmpty()
                .WithMessage("State is required")
                .Length(2, 2)
                .WithMessage("State must be a 2-letter code");

            RuleFor(x => x.ZipCode)
                .NotEmpty()
                .WithMessage("ZIP code is required")
                .Matches("^\\d{5}(-\\d{4})?$")
                .WithMessage("ZIP code must be in format 12345 or 12345-6789");

            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("Country is required")
                .Length(2, 2)
                .WithMessage("Country must be a 2-letter ISO code");
        }
    }
}
