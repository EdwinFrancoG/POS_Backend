using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using POS_Backend;
using POS_Backend.Datos;

namespace POS_Backend.Logica.Servicios;
public record ResultadoProduct(bool Exito, string? Error);
public class ProductService
{
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db) => _db = db;

    //Las consultas
    public async Task<List<Product>> ObtenerTodosAsync(bool soloActivos = true) =>
    await _db.Product
    .Include(p => p.Category)
    .Where(p => !soloActivos || p.Active)
    .OrderBy(p => p.Name)
    .ToListAsync();

    public async Task<Product?> ObtenerporId(int Id) =>
    await _db.Product.Include(p => p.Category)
    .FirstOrDefaultAsync(p => p.Id == Id);

    public async Task<List<Product>> BuscarAsync(string texto)
    {
        texto = texto.Trim().ToLower();
        return await _db.Product
        .Where(p => p.IsActive &&
        (p.Name.ToLower().Contains(texto) ||
        p.Code.ToLower().Contains(texto) ||
        (p.Barcode != null && p.Barcode == texto)))
        .ToListAsync();
    }
    public async Task<List<Product>> ObtenerStockBajo() =>
    await _db.Product
    .Where(p => p.IsActive && p.Stock <= p.MinimunStock)
    .ToListAsync();

    // Todas las operaciones
    // Crear producto
    public async Task<ResultadoProducto> CrearAsync(Product producto)
    {
        var error = Validar(producto);
        if(error !=null) return new(false, error);

        producto.Code = producto.Code.Trim();

        if (await _db.Product.AnyAsync(p => p.Code == producto.Code))
            return new(false, "Ya existe un producto con ese código");

        if (!string.IsNullOrWhiteSpace(producto.Barcode) &&
            await _db.Product.AnyAsync(p => p.Barcode == producto.Barcode))
            return new(false, "Ya existe un producto con ese código de barras");   

         if (!await _db.Category.AnyAsync(c => c.Id == producto.CategoryId))
            return new(false, "La categoría no existe");        

             producto.IsActive = true;
             _db.Product.Add(producto);
            await _db.SaveChangesAsync();
            return new(true, null);
    }
    //Actualizar producto
    public async Task<ResultadoProducto> ActaulizarAsync(Product producto)
    {
        var error = Validar(producto);
        if(error !=null) return new(false, error);

        var existente = await _db.Product.FindAsync(producto.Id);
        if(existente == null)
        return new(false, "El producto no existe");

    producto.Code = producto.Code.Trim();

    if (await _db.Product.AnyAsync(p => p.Code == producto.Code && p.Id != producto.Id))
        return new(false, "Ya existe otro producto con ese código");
    

    if (!string.IsNullOrWhiteSpace(producto.Barcode) &&
        await _db.Product.AnyAsync(p => p.Barcode == producto.Barcode && p.Id != producto.Id))
        return new(false, "Ya existe otro producto con ese código de barras");  

     if (!await _db.Category.AnyAsync(c => c.Id == producto.CategoryId))
        return new(false, "La categoría no existe");

    existente.Code = producto.Code;
    existente.Barcode = producto.Barcode;
    existente.Name = producto.Name;
    existente.Description = producto.Description;
    existente.UnitOfMeasure = producto.UnitOfMeasure;
    existente.CategoryId = producto.CategoryId;
    existente.SupplierId = producto.SupplierId;
    existente.AverageCost = producto.AverageCost;
    existente.SalePrice = producto.SalePrice;
    existente.TaxRate = producto.TaxRate;
    existente.MinimunStock = producto.MinimunStock;

    await _db.SaveChangesAsync();
    return new(true, null);
    }
    //Cambiar estado
    public async Task<ResultadoProducto> CambiarEstadoAsync(int Id, bool Activo)
    {
        var producto = await _db.Product.FindAsync(Id);
        if(producto = null)
        return new(false, "El producto no existe");

        producto.IsActive = Activo;
        await _db.SaveChangesAsync();
        return new(true, null);
    }

    // Las validaciones
    private static string? Validar(Product p)
    {
        if (string.IsNullOrWhiteSpace(p.Code)) return "El codigo es obligatorio";
        if (string.IsNullOrWhiteSpace(p.Name)) return "El nombre es obligatorio";
        if (string.IsNullOrWhiteSpace(p.UnitOfMeasure)) return "La unidad de medida es obligatoria";
        if (p.SalePrice <=0) return "El precio de la venta debe ser mayor a 0";
        if (p.AverageCost < 0) return "El costo no puede ser negativo";
        if (p.TaxRate < 0 || p.TaxRate > 100) return "El impuesto debe estar entre 0 y 100";
        if (p.Stock < 0) return "El stock no puede ser negativo";
        if (p.MinimumStock < 0) return "El stock minimo no puede ser negativo";

        return null;
    }
}