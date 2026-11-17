using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class PurchaseOrderLineMapping : IEntityTypeConfiguration<PurchaseOrderLine>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
        {
            builder.ToTable("PurchaseOrderLine");
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Quantity).HasPrecision(18, 4);
            builder.Property(l => l.ReceivedQuantity).HasPrecision(18, 4);
            builder.Property(l => l.UnitCost).HasPrecision(18, 2);
            builder.Property(l => l.TaxRate).HasPrecision(9, 4);
            builder.Property(l => l.Subtotal).HasPrecision(18, 2);

            builder.HasOne(l => l.PurchaseOrder)
                .WithMany(po => po.PurchaseOrderLines)
                .HasForeignKey(l => l.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
