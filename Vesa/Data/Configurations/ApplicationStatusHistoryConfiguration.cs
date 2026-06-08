using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesa.Models;

namespace Vesa.Data.Configurations;

public class ApplicationStatusHistoryConfiguration : IEntityTypeConfiguration<ApplicationStatusHistory>
{
    public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
    {
        builder.ToTable("ApplicationStatusHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.OldStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.NewStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(x => x.Application)
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ChangedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.ChangedByAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ApplicationId);
    }
}
