using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesa.Models;

namespace Vesa.Data.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.Property(c => c.IsoCode)
            .HasMaxLength(5)
            .IsRequired();

        builder.HasIndex(c => c.IsoCode)
            .IsUnique();

        builder.Property(c => c.FlagEmoji)
            .HasMaxLength(10);
    }
}
