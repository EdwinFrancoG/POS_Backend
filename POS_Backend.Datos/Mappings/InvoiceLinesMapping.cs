using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos.Mappings
{
	public class InvoiceLinesMapping : IEntityTypeConfiguration<InvoiceLines>
	{
		public void Configure(EntityTypeBuilder<InvoiceLines> builder)
		{
			builder.ToTable("InvoiceLines");

			builder.HasKey(i => i.Id);

			builder.Property(i => i.Description)
				.IsRequired()
				.HasMaxLength(250);

			builder.Property(i => i.Quantity).HasColumnType("decimal(18,2)");
			builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
			builder.Property(i => i.Discount).HasColumnType("decimal(18,2)");
			builder.Property(i => i.TaxRate).HasColumnType("decimal(18,2)");
			builder.Property(i => i.Tax).HasColumnType("decimal(18,2)");
			builder.Property(i => i.Total).HasColumnType("decimal(18,2)");
			builder.Property(i => i.UnitCost).HasColumnType("decimal(18,2)");

			builder.HasOne(i => i.Invoice)
				.WithMany(i => i.InvoiceLines)
				.HasForeignKey(i => i.InvoiceId);
		}
	}
}