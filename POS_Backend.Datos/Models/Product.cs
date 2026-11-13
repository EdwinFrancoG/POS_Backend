using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    [Key]
    public int Id { get; set; }

    public string Code { get; set; }
    public string? Barcode { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string UnitOfMeasure { get; set; }

    public int CategoryId { get; set; }
    public int? SupplierId { get; set; }

    public decimal AverageCost { get; set; }
    public decimal SalePrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Stock { get; set; }
    public decimal MinimumStock { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("CategoryId")]
    public Category Category { get; set; }

    public ICollection<InventoryMovement> InventoryMovements { get; set; }
        = new List<InventoryMovement>();
}