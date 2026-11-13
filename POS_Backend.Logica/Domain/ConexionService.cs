using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;

namespace POS_Backend.Logica.Servicios;

public record EstadoConexion(bool Conectado, string? Error);

public class ConexionService
{
    private readonly AppDbContext _db;

    public ConexionService(AppDbContext db) => _db = db;

    public async Task<EstadoConexion> ProbarAsync(CancellationToken ct = default)
    {
        try
        {
            await _db.Database.OpenConnectionAsync(ct);
            await _db.Database.CloseConnectionAsync();
            return new EstadoConexion(true, null);
        }
        catch (Exception ex)
        {
            return new EstadoConexion(false, ex.Message);
        }
    }
}