using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class InvoicesMapping : IEntityTypeConfiguration<Invoices>
    {
        public void Configure(EntityTypeBuilder<Invoices> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.Date)
                .IsRequired();

            builder.Property(i => i.DueDate);

            builder.Property(i => i.PaymentTerm)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(i => i.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(i => i.Subtotal).HasColumnType("decimal(18,2)");
            builder.Property(i => i.Discount).HasColumnType("decimal(18,2)");
            builder.Property(i => i.ExemptAmount).HasColumnType("decimal(18,2)");
            builder.Property(i => i.TaxableAmount).HasColumnType("decimal(18,2)");
            builder.Property(i => i.Tax).HasColumnType("decimal(18,2)");
            builder.Property(i => i.Total).HasColumnType("decimal(18,2)");
            builder.Property(i => i.Balance).HasColumnType("decimal(18,2)");

            builder.Property(i => i.Notes)
                .HasMaxLength(500);

            builder.Property(i => i.VoidReason)
                .HasMaxLength(250);

            builder.Property(i => i.RowVersion)
                .IsRowVersion();

            builder.HasOne(i => i.InvoiceNumberRange)
                .WithMany(r => r.Invoices)
                .HasForeignKey(i => i.InvoiceNumberRangeId);

            builder.HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId);

            builder.HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId);

            builder.HasMany(i => i.InvoiceLines)
                .WithOne(l => l.Invoice)
                .HasForeignKey(l => l.InvoiceId);
        }
    }
}