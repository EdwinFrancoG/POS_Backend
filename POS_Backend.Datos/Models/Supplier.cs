using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS_Backend.Datos.Models
{
    public class Supplier
    {
        [Key] 
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Rtn { get; set; }
        public string? ContactName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int CreditDays { get; set; }
        public bool IsActive { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ICollection<SupplierPayment> SupplierPayments {  get; set; }
    }
}
