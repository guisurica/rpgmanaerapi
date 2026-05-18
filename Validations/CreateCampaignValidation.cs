using FluentValidation;
using rpgmanagerapi.Data.DTOs;

namespace rpgmanagerapi.Validations;

public class CreateCampaignValidation : AbstractValidator<CreateCampaignDTO>
{
    public CreateCampaignValidation()
    {
        RuleFor(cm => cm.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage("Campaign name cannot be null");

        RuleFor(cm => cm.Description)
            .NotEmpty()
            .NotNull()
            .WithMessage("Campaign description cannot be null");

        RuleFor(cm => cm.system)
            .NotEmpty()
            .NotNull()
            .WithMessage("Campaign system cannot be null");

        RuleFor(cm => cm.Name.Length)
            .LessThan(128)
            .WithMessage("Campaign name cannot be this length");
        
        RuleFor(cm => cm.Description.Length)
            .LessThan(255)
            .WithMessage("Campaign name cannot be this length");

        RuleFor(cm => cm.system.Length)
            .LessThan(128)
            .WithMessage("Campaign name cannot be this length");
    }
}