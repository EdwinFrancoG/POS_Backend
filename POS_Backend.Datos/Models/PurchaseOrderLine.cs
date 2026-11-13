using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Text;

namespace POS_Backend.Datos.Models
{
    public class PurchaseOrderLine
    {
        [Key]
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }

        public decimal Quantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Subtotal { get; set; }

        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}
