using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Backend.Model;

public class CashClosings
{
    public int Id { get; set; }
    public DateTime BusinessDate { get; set; }
    public DateTime PeriodStartAt { get; set; }
    public DateTime CutoffAt { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal OpeningAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal CashSales { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal CashCollections { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal CashSupplierPayments { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal OtherExpenses { get; set; }
    public string? OtherExpensesDescription { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal ExpectedAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal CountedAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Difference { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal AmountLeft { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Withdrawal { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal CardPayments { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal TransferPayments { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal CheckPayments { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal CreditSales { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal TotalSales { get; set; }

    public int InvoiceCount { get; set; }
    public string? CountDetail { get; set; }
    public string? Notes { get; set; }
    public CashClosingStatus Status { get; set; }
    public string? VoidReason { get; set; }
    public int UserId { get; set; }

    public Users User { get; set; } = null!;
}