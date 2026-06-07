using System;
using System.Collections.Generic;

namespace Vesa.Models;

public class AppointmentSlot
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public Guid CountryId { get; set; }
    public int MaxCapacity { get; set; }
    public int BookedCount { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public Country Country { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
