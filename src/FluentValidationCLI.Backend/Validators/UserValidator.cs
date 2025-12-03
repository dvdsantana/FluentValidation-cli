using FluentValidation;
using FluentValidationCLI.Backend.Models;

namespace FluentValidationCLI.Backend.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required").Length(3, 20).WithMessage("Username must be between 3 and 20 chars");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Age).GreaterThan(18).LessThan(100);
    }
}
