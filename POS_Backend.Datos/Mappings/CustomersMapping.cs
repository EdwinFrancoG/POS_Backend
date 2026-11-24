using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class CustomersMapping : IEntityTypeConfiguration<Customers>
    {
        public void Configure(EntityTypeBuilder<Customers> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Rtn)
                .HasMaxLength(20);

            builder.Property(c => c.Phone)
                .HasMaxLength(20);

            builder.Property(c => c.Email)
                .HasMaxLength(150);

            builder.Property(c => c.Address)
                .HasMaxLength(250);

            builder.Property(c => c.CreditLimit)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.CreditDays)
                .IsRequired();

            builder.Property(c => c.IsActive)
                .IsRequired();

            builder.HasMany(c => c.Invoices)
                .WithOne(i => i.Customer)
                .HasForeignKey(i => i.CustomerId);
        }
    }
}