using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesa.Models;

namespace Vesa.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.FileUrl)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(d => d.DocumentType)
            .HasConversion<int>();

        builder.HasOne(d => d.VisaApplication)
            .WithMany(a => a.Documents)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
