using System;
using FluentValidation;
using Vesa.DTOs.Appointments;

namespace Vesa.Validators;

public class CreateAppointmentSlotRequestValidator : AbstractValidator<CreateAppointmentSlotRequest>
{
    public CreateAppointmentSlotRequestValidator()
    {
        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("Country ID is required.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Slot date cannot be in the past.");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Max capacity must be greater than 0.");
    }
}
