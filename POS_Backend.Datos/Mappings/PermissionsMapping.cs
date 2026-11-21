using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace POS_Backend.Datos.Mappings
{
    public class PermissionsMapping : IEntityTypeConfiguration<Permissions>
    {
        public void Configure(EntityTypeBuilder<Permissions> builder)
        {
            builder.ToTable("Permissions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(x => x.Description)
                .HasMaxLength(250);
            builder.Property(x => x.Module)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(p => p.Code)
                .IsUnique();
        }
    }
}
