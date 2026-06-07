using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesa.Models;

namespace Vesa.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Currency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.TransactionReference)
            .HasMaxLength(255);

        builder.Property(p => p.Status)
            .HasConversion<int>();

        builder.Property(p => p.Method)
            .HasConversion<int?>();

        builder.HasOne(p => p.VisaApplication)
            .WithMany()
            .HasForeignKey(p => p.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Applicant)
            .WithMany()
            .HasForeignKey(p => p.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.ApplicationId)
            .IsUnique();

        builder.HasIndex(p => p.ApplicantId);
    }
}
