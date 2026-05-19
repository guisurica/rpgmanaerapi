using FluentValidation;
using rpgmanagerapi.Data.DTOs;

namespace rpgmanagerapi.Validations;

public class LoginUserValidation : AbstractValidator<LoginUserDTO>
{
    public LoginUserValidation()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("invalid email");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("password is required");
            
    }
}