using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace POS_Backend.Datos.Mappings
{
    public class RolesMapping : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.Description)
                .HasMaxLength(250);
            builder.HasIndex(p => p.Name)
                .IsUnique();
        }
    
    }
}
