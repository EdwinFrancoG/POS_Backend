using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Backend.Model;

public class Invoices
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int InvoiceNumberRangeId { get; set; }
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? DueDate { get; set; }
    public PaymentTerm PaymentTerm { get; set; }
    public InvoiceStatus Status { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal Subtotal { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Discount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal ExemptAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal TaxableAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Tax { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Total { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Balance { get; set; }

    public string? Notes { get; set; }
    public string? VoidReason { get; set; }
    public int UserId { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    public InvoiceNumberRanges InvoiceNumberRange { get; set; } = null!;
    public Customers Customer { get; set; } = null!;
    public Users User { get; set; } = null!;
    public ICollection<InvoiceLines> InvoiceLines { get; set; } = new List<InvoiceLines>();
    public ICollection<Payments> Payments { get; set; } = new List<Payments>();
}