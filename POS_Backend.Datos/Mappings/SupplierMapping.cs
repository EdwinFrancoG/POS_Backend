using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class SupplierMapping : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Supplier");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Code).IsRequired().HasMaxLength(50);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.Rtn).HasMaxLength(20);
            builder.Property(s => s.ContactName).HasMaxLength(150);
            builder.Property(s => s.Phone).HasMaxLength(30);
            builder.Property(s => s.Email).HasMaxLength(150);
            builder.Property(s => s.Address).HasMaxLength(300);

            builder.HasIndex(s => s.Code).IsUnique();
        }
    }
}
