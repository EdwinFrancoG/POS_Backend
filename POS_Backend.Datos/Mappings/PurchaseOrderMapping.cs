using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class PurchaseOrderMapping : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.ToTable("PurchaseOrder");
            builder.HasKey(po => po.Id);

            builder.Property(po => po.Number).IsRequired().HasMaxLength(30);
            builder.Property(po => po.Status).IsRequired().HasMaxLength(30);
            builder.Property(po => po.Notes).HasMaxLength(500);

            builder.Property(po => po.Subtotal).HasPrecision(18, 2);
            builder.Property(po => po.Tax).HasPrecision(18, 2);
            builder.Property(po => po.Total).HasPrecision(18, 2);
            builder.Property(po => po.AmountReceived).HasPrecision(18, 2);
            builder.Property(po => po.AmoutPaid).HasPrecision(18, 2);

            builder.Property(po => po.RowVersion).IsRowVersion();

            builder.HasIndex(po => po.Number).IsUnique();

            builder.HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
