using System;

namespace Vesa.DTOs.Appointments;

public class AppointmentResponse
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid SlotId { get; set; }
    public string ApplicantId { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime BookedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? Notes { get; set; }
    public bool IsReminderSent { get; set; }

    // Slot details
    public DateOnly SlotDate { get; set; }
    public TimeOnly SlotTime { get; set; }
    public string CountryName { get; set; } = string.Empty;
}
