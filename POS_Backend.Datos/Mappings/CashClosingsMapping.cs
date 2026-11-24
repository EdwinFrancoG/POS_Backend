using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
    public class CashClosingsMapping : IEntityTypeConfiguration<CashClosings>
    {
        public void Configure(EntityTypeBuilder<CashClosings> builder)
        {
            builder.ToTable("CashClosings");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.BusinessDate)
                .IsRequired();

            builder.Property(c => c.PeriodStartAt)
                .IsRequired();

            builder.Property(c => c.CutoffAt)
                .IsRequired();

            builder.Property(c => c.OpeningAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CashSales).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CashCollections).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CashSupplierPayments).HasColumnType("decimal(18,2)");
            builder.Property(c => c.OtherExpenses).HasColumnType("decimal(18,2)");
            builder.Property(c => c.ExpectedAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CountedAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.Difference).HasColumnType("decimal(18,2)");
            builder.Property(c => c.AmountLeft).HasColumnType("decimal(18,2)");
            builder.Property(c => c.Withdrawal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CardPayments).HasColumnType("decimal(18,2)");
            builder.Property(c => c.TransferPayments).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CheckPayments).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CreditSales).HasColumnType("decimal(18,2)");
            builder.Property(c => c.TotalSales).HasColumnType("decimal(18,2)");

            builder.Property(c => c.OtherExpensesDescription)
                .HasMaxLength(250);

            builder.Property(c => c.CountDetail)
                .HasMaxLength(500);

            builder.Property(c => c.Notes)
                .HasMaxLength(500);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(c => c.VoidReason)
                .HasMaxLength(250);

            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);
        }
    }
}