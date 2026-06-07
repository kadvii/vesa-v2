using FluentValidation;
using Vesa.DTOs.VisaTypes;

namespace Vesa.Validators;

public class CreateVisaTypeRequestValidator : AbstractValidator<CreateVisaTypeRequest>
{
    public CreateVisaTypeRequestValidator()
    {
        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("Country ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Visa Type Name is required.")
            .MaximumLength(100).WithMessage("Visa Type Name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.ProcessingDays)
            .GreaterThan(0).WithMessage("Processing Days must be greater than 0.");

        RuleFor(x => x.FeeAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Fee Amount cannot be negative.");
    }
}
