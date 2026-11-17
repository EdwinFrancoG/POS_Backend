using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace POS_Backend.Datos.Mappings
{
    public class ProductMapping : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Barcode)
                .HasMaxLength(50);

            builder.Property(x => x.Name)
                .HasMaxLength(75)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.UnitOfMeasure)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.AverageCost)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.SalePrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.TaxRate)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Stock)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.MinimumStock)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Supplier>()
                .WithMany()
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
