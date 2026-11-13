using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace POS_Backend.Datos.Models
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string Source { get; set; }
        public string Status { get; set; }

        public int? InvoiceId { get; set; }
        public int? PurchaseOrderId { get; set; }
        public int? PaymentId { get; set; }
        public int? InventoryMovementId { get; set; }
        public int? SupplierPaymentId { get; set; }
        public int? ReversedEntryId { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public int UserId { get; set; }

        public ICollection<JournalEntryLine> JournalEntryLines { get; set; }
            = new List<JournalEntryLine>();

        [ForeignKey("ReversedEntryId")]
        public JournalEntry? ReversedEntry { get; set; }
    }
}
