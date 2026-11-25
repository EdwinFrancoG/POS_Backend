using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Logica.Services;

public class SupplierService : ISupplierService
{
    private readonly AppDbContext _db;

    public SupplierService(AppDbContext db) => _db = db;

    public async Task<List<Supplier>> GetAllAsync(bool onlyActive, CancellationToken ct = default)
    {
        var query = _db.Suppliers.AsNoTracking();

        if (onlyActive)
            query = query.Where(s => s.IsActive);

        return await query.OrderBy(s => s.Name).ToListAsync(ct);
    }

    public async Task<Supplier?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Suppliers.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId, CancellationToken ct = default) =>
        await _db.Suppliers.AnyAsync(
            s => s.Code == code && (excludeId == null || s.Id != excludeId), ct);

    public async Task<bool> ExistsByRtnAsync(string rtn, int? excludeId, CancellationToken ct = default) =>
        await _db.Suppliers.AnyAsync(
            s => s.Rtn == rtn && (excludeId == null || s.Id != excludeId), ct);

    public async Task<Supplier> AddAsync(Supplier supplier, CancellationToken ct = default)
    {
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync(ct);
        return supplier;
    }

    public async Task UpdateAsync(Supplier supplier, CancellationToken ct = default)
    {
        _db.Suppliers.Update(supplier);
        await _db.SaveChangesAsync(ct);
    }
}