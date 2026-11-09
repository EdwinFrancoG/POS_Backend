using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Backend.Model;

public class Customers
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Rtn { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CreditLimit { get; set; }
    public int CreditDays { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Invoices> Invoices { get; set; } = new List<Invoices>();
}