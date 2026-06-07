using FluentValidation;
using Vesa.DTOs.VisaTypes;

namespace Vesa.Validators;

public class CreateCountryRequestValidator : AbstractValidator<CreateCountryRequest>
{
    public CreateCountryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Country Name is required.")
            .MaximumLength(100).WithMessage("Country Name cannot exceed 100 characters.");

        RuleFor(x => x.IsoCode)
            .NotEmpty().WithMessage("ISO Code is required.")
            .MaximumLength(5).WithMessage("ISO Code cannot exceed 5 characters.");

        RuleFor(x => x.FlagEmoji)
            .MaximumLength(10).WithMessage("Flag Emoji cannot exceed 10 characters.");
    }
}
