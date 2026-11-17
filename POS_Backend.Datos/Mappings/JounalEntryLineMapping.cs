using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class JournalEntryLineMapping : IEntityTypeConfiguration<JournalEntryLine>
    {
        public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
        {
            builder.ToTable("JournalEntryLine");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Debit)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.Credit)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.Description)
                .HasMaxLength(250);

            builder.HasOne(j => j.JournalEntry)
                .WithMany(e => e.JournalEntryLines)
                .HasForeignKey(j => j.JournalEntryId);

            builder.HasOne(j => j.LedgerAccount)
                .WithMany(l => l.JournalEntryLines)
                .HasForeignKey(j => j.LedgerAccountId);
        }
    }
}