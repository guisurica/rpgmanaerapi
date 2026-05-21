using FluentValidation;
using rpgmanagerapi.Data.DTOs;

namespace rpgmanagerapi.Validations;

public class CreateReportValidation : AbstractValidator<CreateReportDTO>
{
    public CreateReportValidation()
    {
        RuleFor(r => r.text)
            .NotEmpty().WithMessage("Report text is required");

        RuleFor(r => r.name)
            .NotEmpty().WithMessage("Report name is required");
    }
}