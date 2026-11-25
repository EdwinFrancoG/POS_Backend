namespace POS_Backend.Logica.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync(bool onlyActive, CancellationToken ct = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Product>> SearchActiveAsync(string text, CancellationToken ct = default);
    Task<List<Product>> GetLowStockAsync(CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludeId, CancellationToken ct = default);
    Task<bool> ExistsByBarcodeAsync(string barcode, int? excludeId, CancellationToken ct = default);
    Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default);
    Task<bool> SupplierExistsAsync(int supplierId, CancellationToken ct = default);
    Task<Product> AddAsync(Product product, CancellationToken ct = default);
    Task<Product> UpdateAsync(Product product, CancellationToken ct = default);
}