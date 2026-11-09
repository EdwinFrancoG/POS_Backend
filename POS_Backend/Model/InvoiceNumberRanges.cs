using System.ComponentModel.DataAnnotations;

namespace POS_Backend.Model;

public class InvoiceNumberRanges
{
    public int Id { get; set; }
    public string? Cai { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public long StartNumber { get; set; }
    public long EndNumber { get; set; }
    public long CurrentNumber { get; set; }
    public DateTime? IssueDeadline { get; set; }
    public bool IsActive { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    public ICollection<Invoices> Invoices { get; set; } = new List<Invoices>();
}