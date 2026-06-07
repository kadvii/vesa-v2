using FluentValidation;
using Vesa.DTOs.Appointments;

namespace Vesa.Validators;

public class BookAppointmentRequestValidator : AbstractValidator<BookAppointmentRequest>
{
    public BookAppointmentRequestValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("Application ID is required.");

        RuleFor(x => x.SlotId)
            .NotEmpty().WithMessage("Slot ID is required.");
    }
}
