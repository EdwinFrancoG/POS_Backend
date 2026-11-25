using POS_Backend.Datos.Models;

namespace POS_Backend.Logica.Services;

public interface ISupplierService
{
    Task<List<Supplier>> GetAllAsync(bool onlyActive, CancellationToken ct = default);
    Task<Supplier?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludeId, CancellationToken ct = default);
    Task<bool> ExistsByRtnAsync(string rtn, int? excludeId, CancellationToken ct = default);
    Task<Supplier> AddAsync(Supplier supplier, CancellationToken ct = default);
    Task UpdateAsync(Supplier supplier, CancellationToken ct = default);
}