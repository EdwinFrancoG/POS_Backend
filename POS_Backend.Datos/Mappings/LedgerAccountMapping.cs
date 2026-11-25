using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class LedgerAccountMapping : IEntityTypeConfiguration<LedgerAccount>
    {
        public void Configure(EntityTypeBuilder<LedgerAccount> builder)
        {
            builder.ToTable("LedgerAccount");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Code)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.Type)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(l => l.Nature)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(l => l.Level)
                .IsRequired();

            builder.Property(l => l.AllowsPosting)
                .IsRequired();

            builder.Property(l => l.IsActive)
                .IsRequired();

            builder.HasOne(l => l.ParentAccount)
                .WithMany(l => l.SubAccounts)
                .HasForeignKey("ParentAccountId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}