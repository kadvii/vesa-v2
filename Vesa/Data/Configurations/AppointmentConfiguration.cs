using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesa.Models;

namespace Vesa.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .HasConversion<int>();

        builder.Property(a => a.Notes)
            .HasMaxLength(2000);

        builder.HasOne(a => a.VisaApplication)
            .WithMany()
            .HasForeignKey(a => a.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.AppointmentSlot)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.SlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Applicant)
            .WithMany()
            .HasForeignKey(a => a.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.ApplicationId);
        builder.HasIndex(a => a.ApplicantId);
        builder.HasIndex(a => a.SlotId);
    }
}
