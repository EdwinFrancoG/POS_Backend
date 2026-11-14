using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class SupplierPaymentMapping : IEntityTypeConfiguration<SupplierPayment>
    {
        public void Configure(EntityTypeBuilder<SupplierPayment> builder)
        {
            builder.ToTable("SupplierPayment");
            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.Number).IsRequired().HasMaxLength(30);
            builder.Property(sp => sp.Amount).HasPrecision(18, 2);
            builder.Property(sp => sp.Method).IsRequired().HasMaxLength(30);
            builder.Property(sp => sp.Reference).HasMaxLength(100);
            builder.Property(sp => sp.Notes).HasMaxLength(500);
            builder.Property(sp => sp.Status).IsRequired().HasMaxLength(30);
            builder.Property(sp => sp.VoidReason).HasMaxLength(300);

            builder.HasIndex(sp => sp.Number).IsUnique();

            builder.HasOne(sp => sp.Supplier)
                .WithMany(s => s.SupplierPayments)
                .HasForeignKey(sp => sp.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
