using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace POS_Backend.Datos.Mappings
{
    public class PaymentsMapping : IEntityTypeConfiguration<Payments>
    {
        public void Configure(EntityTypeBuilder<Payments> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Date)
                .IsRequired();
            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(x => x.Method)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.Reference)
                .HasMaxLength(100);

            builder.HasOne(p => p.Invoice)
              .WithMany()
              .HasForeignKey(p => p.InvoiceId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
