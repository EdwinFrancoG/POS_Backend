using Microsoft.EntityFrameworkCore;

namespace POS_Backend.Datos;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

}