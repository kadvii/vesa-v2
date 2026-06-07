using System;

namespace Vesa.DTOs.Appointments;

public class BookAppointmentRequest
{
    public Guid ApplicationId { get; set; }
    public Guid SlotId { get; set; }
}
