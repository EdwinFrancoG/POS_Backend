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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Carga automáticamente todas las clases IEntityTypeConfiguration<T>
        // de la carpeta Mappings, así no hay que registrarlas una por una.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
