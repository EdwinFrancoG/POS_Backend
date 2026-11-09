using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Backend.Model;

public class InvoiceLines
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")] public decimal Quantity { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal UnitPrice { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Discount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal TaxRate { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Tax { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Total { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal UnitCost { get; set; }

    public Invoices Invoice { get; set; } = null!;
    // public Products Product { get; set; } = null!;  // cuando exista la clase Products
}