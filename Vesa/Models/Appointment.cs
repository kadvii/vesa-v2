using System;
using Vesa.Models.Enums;

namespace Vesa.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid SlotId { get; set; }
    public string ApplicantId { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; }
    public DateTime BookedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? Notes { get; set; }
    public bool IsReminderSent { get; set; }

    // Navigation properties
    public VisaApplication VisaApplication { get; set; } = null!;
    public AppointmentSlot AppointmentSlot { get; set; } = null!;
    public AppUser Applicant { get; set; } = null!;
}
