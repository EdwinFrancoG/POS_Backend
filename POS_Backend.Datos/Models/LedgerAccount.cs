using POS_Backend.Datos.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class LedgerAccount
{
    [Key]
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }

    public string Type { get; set; }
    public string Nature { get; set; }
    public int Level { get; set; }
    public bool AllowsPosting { get; set; }
    public bool IsActive { get; set; }


    [ForeignKey("ParentAccountId")]
    public LedgerAccount? ParentAccount { get; set; }
    ICollection<LedgerAccount> SubAccounts { get; set; }
    =new List<LedgerAccount>();

    public ICollection<JournalEntryLine> JournalEntryLines { get; set; }
    = new List<JournalEntryLine>();

    public ICollection<AccountMapping> AccountMappings { get; set; }
   = new List<AccountMapping>();

}

   