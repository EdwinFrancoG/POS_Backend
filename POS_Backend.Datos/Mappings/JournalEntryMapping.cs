using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class JournalEntryMapping : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            builder.ToTable("JournalEntry");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Number)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(j => j.Date)
                .IsRequired();

            builder.Property(j => j.Description)
                .HasMaxLength(250);

            builder.Property(j => j.Source)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(j => j.Status)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(j => j.TotalDebit)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.TotalCredit)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(j => j.JournalEntryLines)
                .WithOne(l => l.JournalEntry)
                .HasForeignKey(l => l.JournalEntryId);

            builder.HasOne(j => j.ReversedEntry)
                .WithMany()
                .HasForeignKey(j => j.ReversedEntryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}