using FluentValidation;

namespace SampleApp.Models
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {

            RuleFor(x => x.Id)
                .NotNull()
                .WithMessage("User ID is required");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Please provide a valid email address")
                .MaximumLength(255)
                .WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.Age)
                .InclusiveBetween(18, 120)
                .WithMessage("Age must be between 18 and 120");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .Length(2, 100)
                .WithMessage("Name must be between 2 and 100 characters");
        }
    }
}
