using System;

namespace Vesa.DTOs.Appointments;

public class AppointmentSlotResponse
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int BookedCount { get; set; }
    public bool IsActive { get; set; }
}
