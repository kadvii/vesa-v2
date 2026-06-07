using System;

namespace Vesa.DTOs.Appointments;

public class CreateAppointmentSlotRequest
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public Guid CountryId { get; set; }
    public int MaxCapacity { get; set; }
}
