using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using POS_Backend.Datos.Mappings;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos;

public class AppDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.ApplyConfiguration(new ProductMapping());
        modelBuilder.ApplyConfiguration(new SupplierMapping());
        modelBuilder.ApplyConfiguration(new SupplierPaymentMapping());
        modelBuilder.ApplyConfiguration(new SupplierPaymentAllocationMapping());
        modelBuilder.ApplyConfiguration(new PurchaseOrderMapping());
        modelBuilder.ApplyConfiguration(new PurchaseOrderLineMapping());
    }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<SupplierPayment> SupplierPayments { get; set; }
    public DbSet<SupplierPaymentAllocation> SupplierPaymentAllocations { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }

}