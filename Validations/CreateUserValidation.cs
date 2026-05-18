using FluentValidation;
using rpgmanagerapi.Data.DTOs;

namespace rpgmanagerapi.Validations;

public class CreateUserValidation : AbstractValidator<CreateUserDTO>
{
    public CreateUserValidation()
    {
        RuleFor(u => u.Email)
            .EmailAddress()
            .WithMessage("invalid email");

        RuleFor(u => u.Username)
            .Empty()
            .WithMessage("Invalid username");

        RuleFor(u => u.Username.Length)
            .LessThan(128)
            .WithMessage("Invalid username");

        RuleFor(u => u.Username.Length)
            .LessThan(128)
            .WithMessage("invalid username");

        RuleFor(u => u.Password)
            .NotEqual(u => u.CPassword)
            .WithMessage("Passwords doesnt match");

        RuleFor(u => u.Password.Length)
            .LessThan(255)
            .WithMessage("Password is to long");
    }
}