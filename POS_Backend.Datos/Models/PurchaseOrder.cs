using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Text;

namespace POS_Backend.Datos.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }
        public int SupplierId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string? Notes { get; set; }
        public int? CreatedById { get; set; }
        public int? ApprovedById { get; set; }
        public DateTime? AprovedAt { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal AmoutPaid { get; set; }
        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }
        public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public ICollection<SupplierPaymentAllocation> PaymentAllocations { get; set; }
    }
}
