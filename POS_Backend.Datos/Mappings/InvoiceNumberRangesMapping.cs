using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class InvoiceNumberRangesMapping : IEntityTypeConfiguration<InvoiceNumberRanges>
    {
        public void Configure(EntityTypeBuilder<InvoiceNumberRanges> builder)
        {
            builder.ToTable("InvoiceNumberRanges");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Cai)
                .HasMaxLength(100);

            builder.Property(i => i.Prefix)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(i => i.StartNumber)
                .IsRequired();

            builder.Property(i => i.EndNumber)
                .IsRequired();

            builder.Property(i => i.CurrentNumber)
                .IsRequired();

            builder.Property(i => i.IssueDeadline);

            builder.Property(i => i.IsActive)
                .IsRequired();

            builder.Property(i => i.RowVersion)
                .IsRowVersion();

            builder.HasMany(i => i.Invoices)
                .WithOne(i => i.InvoiceNumberRange)
                .HasForeignKey(i => i.InvoiceNumberRangeId);
        }
    }
}