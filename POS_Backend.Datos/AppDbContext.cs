using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos;

public class AppDbContext : DbContext
{
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<LedgerAccount> LedgerAccounts => Set<LedgerAccount>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

}