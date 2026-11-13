using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace POS_Backend.Datos.Models
{
    public class SupplierPaymentAllocation
    {
        [Key]
        public int Id { get; set; }

        public int SupplierPaymentId { get; set; }
        public int PurchaseOrderId { get; set; }
        public decimal Amount { get; set; }

        [ForeignKey("SupplierPaymentId")]
        public SupplierPayment SupplierPayment { get; set; }

        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}
