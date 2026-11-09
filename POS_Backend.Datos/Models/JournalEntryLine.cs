using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace POS_Backend.Datos.Models
{
    public class JournalEntryLine
    {
        [Key]
        public int Id { get; set; }

        public int JournalEntryId { get; set; }
        public int LedgerAccountId { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public string? Description { get; set; }

        [ForeignKey("JournalEntryId")]
        public JournalEntry JournalEntry { get; set; }

        [ForeignKey("LedgerAccountId")]
        public LedgerAccount LedgerAccount { get; set; }
    }
}