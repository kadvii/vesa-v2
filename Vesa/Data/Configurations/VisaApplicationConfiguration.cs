using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesa.Models;

namespace Vesa.Data.Configurations;

public class VisaApplicationConfiguration : IEntityTypeConfiguration<VisaApplication>
{
    public void Configure(EntityTypeBuilder<VisaApplication> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PassportNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.AdminNotes)
            .HasMaxLength(2000);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(2000);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.HasOne(x => x.Applicant)
            .WithMany()
            .HasForeignKey(x => x.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.VisaType)
            .WithMany(v => v.VisaApplications)
            .HasForeignKey(x => x.VisaTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Country)
            .WithMany(c => c.VisaApplications)
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReviewedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.ReviewedByAdminId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
