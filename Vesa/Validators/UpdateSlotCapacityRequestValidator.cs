using FluentValidation;
using Vesa.DTOs.Appointments;

namespace Vesa.Validators;

public class UpdateSlotCapacityRequestValidator : AbstractValidator<UpdateSlotCapacityRequest>
{
    public UpdateSlotCapacityRequestValidator()
    {
        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Max capacity must be greater than 0.");
    }
}
