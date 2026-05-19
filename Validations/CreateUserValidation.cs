using FluentValidation;
using rpgmanagerapi.Data.DTOs;

namespace rpgmanagerapi.Validations;

public class CreateUserValidation : AbstractValidator<CreateUserDTO>
{
    public CreateUserValidation()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email");

        RuleFor(u => u.Username)
            .NotEmpty().WithMessage("Username is required")
            .Length(3, 128).WithMessage("Username must be between 3 and 128 characters");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required")
            .MaximumLength(255).WithMessage("Password is too long")
            .Equal(u => u.CPassword).WithMessage("Passwords don't match");
    }
}