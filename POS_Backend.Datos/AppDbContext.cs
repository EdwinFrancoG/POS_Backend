using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos.Models;

namespace POS_Backend.Datos;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customers> Customers { get; set; }

}
