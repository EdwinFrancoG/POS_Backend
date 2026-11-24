using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class AuditLogsMapping : IEntityTypeConfiguration<AuditLogs>
    {
        public void Configure(EntityTypeBuilder<AuditLogs> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(a => a.Module)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Entity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.Changes)
                .HasColumnType("nvarchar(max)");

            builder.Property(a => a.Date)
                .IsRequired();
        }
    }
}