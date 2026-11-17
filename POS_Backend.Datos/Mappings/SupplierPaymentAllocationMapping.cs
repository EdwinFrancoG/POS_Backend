using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class SupplierPaymentAllocationMapping : IEntityTypeConfiguration<SupplierPaymentAllocation>
    {
        public void Configure(EntityTypeBuilder<SupplierPaymentAllocation> builder)
        {
            builder.ToTable("SupplierPaymentAllocation");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Amount).HasPrecision(18, 2);

            builder.HasOne(a => a.SupplierPayment)
                .WithMany(sp => sp.Allocations)
                .HasForeignKey(a => a.SupplierPaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.PurchaseOrder)
                .WithMany(po => po.PaymentAllocations)
                .HasForeignKey(a => a.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}