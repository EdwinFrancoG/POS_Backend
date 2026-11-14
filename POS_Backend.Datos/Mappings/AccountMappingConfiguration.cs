using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class AccountMappingConfiguration : IEntityTypeConfiguration<AccountMapping>
    {
        public void Configure(EntityTypeBuilder<AccountMapping> builder)
        {
            builder.ToTable("AccountMapping");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Key)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(a => a.LedgerAccount)
                .WithMany(l => l.AccountMappings)
                .HasForeignKey(a => a.LedgerAccountId);
        }
    }
}
