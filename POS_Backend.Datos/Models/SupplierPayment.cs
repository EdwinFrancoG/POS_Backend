using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace POS_Backend.Datos.Models
{
    public class SupplierPayment
    {
        [Key]
        public int Id { get; set; }

        public string Number { get; set; }
        public int SupplierId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }

        public int? SourceLedgerAccountId { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; }
        public string? VoidReason { get; set; }

        public int UserId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        public ICollection<SupplierPaymentAllocation> Allocations { get; set; }
    }
}
