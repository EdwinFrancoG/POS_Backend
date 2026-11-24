using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class InvoicesMapping : IEntityTypeConfiguration<Invoices>
    {
        public void Configure(EntityTypeBuilder<Invoices> builder)
        {
            builder.HasKey(i => i.Id);

            // Una factura no debe desaparecer al borrar su cliente, usuario o rango CAI.
            builder.HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.User)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.InvoiceNumberRange)
                .WithMany(r => r.Invoices)
                .HasForeignKey(i => i.InvoiceNumberRangeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.InvoiceLines)
                .WithOne(l => l.Invoice)
                .HasForeignKey(l => l.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
