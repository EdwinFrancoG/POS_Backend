using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace POS_Backend.Datos.Mappings
{
    public class UsersMapping : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Username)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.FullName)
                .HasMaxLength(100);
            builder.Property(x => x.Email)
               .HasMaxLength(100);
            builder.Property(x => x.PasswordHash)
                .HasMaxLength(255)
                .IsRequired();
            

            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
        }
    
    }
}
