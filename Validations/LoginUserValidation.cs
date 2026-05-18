using FluentValidation;
using rpgmanagerapi.Data.DTOs;

namespace rpgmanagerapi.Validations;

public class LoginUserValidation : AbstractValidator<LoginUserDTO>
{
    public LoginUserValidation()
    {
        RuleFor(u => u.Email)
            .EmailAddress()
            .WithMessage("invalid email");

        RuleFor(u => u.Password)
            .Empty()
            .WithMessage("invalid password");
    }
}