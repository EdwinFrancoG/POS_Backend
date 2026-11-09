using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace POS_Backend.Datos.Models
{
    public class AccountMapping
    {
        [Key]
        public int Id { get; set; }

        public string Key { get; set; }

        public int LedgerAccountId { get; set; }

        [ForeignKey("LedgerAccountId")]
        public LedgerAccount LedgerAccount { get; set; }
    }
}
