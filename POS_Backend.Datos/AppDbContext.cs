using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Catálogos
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customers> Customers { get; set; }

    // Proveedores y compras
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<SupplierPayment> SupplierPayments { get; set; }
    public DbSet<SupplierPaymentAllocation> SupplierPaymentAllocations { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }

    // Inventario
    public DbSet<InventoryMovement> InventoryMovements { get; set; }

    // Contabilidad
    public DbSet<LedgerAccount> LedgerAccounts { get; set; }
    public DbSet<AccountMapping> AccountMappings { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<JournalEntryLine> JournalEntryLines { get; set; }

    // Ventas
    public DbSet<Invoices> Invoices { get; set; }
    public DbSet<InvoiceLines> InvoiceLines { get; set; }
    public DbSet<InvoiceNumberRanges> InvoiceNumberRanges { get; set; }
    public DbSet<Payments> Payments { get; set; }
    public DbSet<CashClosings> CashClosings { get; set; }

    // Seguridad y auditoría
    public DbSet<Users> Users { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<Permissions> Permissions { get; set; }
    public DbSet<UserRoles> UserRoles { get; set; }
    public DbSet<RolesPermissions> RolesPermissions { get; set; }
    public DbSet<AuditLogs> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Carga automáticamente todas las clases IEntityTypeConfiguration<T>
        // de la carpeta Mappings, así no hay que registrarlas una por una.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
