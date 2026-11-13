using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class InventoryMovement
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Type { get; set; }
    public DateTime Date { get; set; }

    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }

    public string? ResultingStock { get; set; }

    public int? InvoiceId { get; set; }
    public int? PurchaseOrderId { get; set; }

    public string? Notes { get; set; }

    public int UserId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}