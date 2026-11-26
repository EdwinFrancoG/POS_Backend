using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;

namespace POS_Backend.Logica.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db) => _db = db;

    public async Task<List<Product>> GetAllAsync(bool onlyActive, CancellationToken ct = default)
    {
        IQueryable<Product> query = _db.Products.AsNoTracking().Include(p => p.Category);

        if (onlyActive)
            query = query.Where(p => p.IsActive);

        return await query.OrderBy(p => p.Name).ToListAsync(ct);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<List<Product>> SearchActiveAsync(string text, CancellationToken ct = default)
    {
        var lowered = text.ToLower();

        return await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive &&
                (p.Name.ToLower().Contains(lowered) ||
                 p.Code.ToLower().Contains(lowered) ||
                 (p.Barcode != null && p.Barcode == text)))
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<List<Product>> GetLowStockAsync(CancellationToken ct = default) =>
        await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.Stock <= p.MinimumStock)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId, CancellationToken ct = default) =>
        await _db.Products.AnyAsync(
            p => p.Code == code && (excludeId == null || p.Id != excludeId), ct);

    public async Task<bool> ExistsByBarcodeAsync(string barcode, int? excludeId, CancellationToken ct = default) =>
        await _db.Products.AnyAsync(
            p => p.Barcode == barcode && (excludeId == null || p.Id != excludeId), ct);

    public async Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default) =>
        await _db.Categories.AnyAsync(c => c.Id == categoryId, ct);

    public async Task<bool> SupplierExistsAsync(int supplierId, CancellationToken ct = default) =>
        await _db.Suppliers.AnyAsync(s => s.Id == supplierId, ct);

    public async Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
        return await LoadSavedAsync(product.Id, ct);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
        return await LoadSavedAsync(product.Id, ct);
    }

    private async Task<Product> LoadSavedAsync(int id, CancellationToken ct) =>
        await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstAsync(p => p.Id == id, ct);
}