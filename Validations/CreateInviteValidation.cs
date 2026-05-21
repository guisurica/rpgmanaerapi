using FluentValidation;
using rpgmanagerapi.Data.DTOs;

namespace rpgmanagerapi.Validations;

public class CreateInviteValidation : AbstractValidator<CreateInviteDTO>
{
    public CreateInviteValidation()
    {
        RuleFor(i => i.campaignId)
            .NotEmpty().WithMessage("Campaign id is required");

        RuleFor(i => i.userWantInviteUsername)
            .NotEmpty().WithMessage("A user need to receive this invite");
    }
}