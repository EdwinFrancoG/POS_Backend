using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class InventoryMovementMapping : IEntityTypeConfiguration<InventoryMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryMovement> builder)
        {
            builder.ToTable("InventoryMovement");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Type)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(i => i.Date)
                .IsRequired();

            builder.Property(i => i.Quantity)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.UnitCost)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.ResultingStock)
                .HasMaxLength(50);

            builder.Property(i => i.Notes)
                .HasMaxLength(250);

            builder.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId);
        }
    }
}