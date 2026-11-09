using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Backend.Model;

public class Payments
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public DateTime Date { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string? Reference { get; set; }
    public int UserId { get; set; }

    public Invoices Invoice { get; set; } = null!;
    public Users User { get; set; } = null!;
}