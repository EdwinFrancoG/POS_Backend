using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;

namespace POS_Backend.Logica.Services;

public class DatabaseConnectionService : IDatabaseConnectionService
{
    private readonly AppDbContext _db;

    public DatabaseConnectionService(AppDbContext db) => _db = db;

    public async Task VerifyConnectionAsync(CancellationToken ct = default)
    {
        await _db.Database.OpenConnectionAsync(ct);
        await _db.Database.CloseConnectionAsync();
    }
}