using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Logica.Domain;

public class ProductDomain
{
    private readonly AppDbContext _db;

    public ProductDomain(AppDbContext db)
    {
        _db = db;
    }

    // Validaciones internas
    private static string? Validar(Product producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Code))
        {
            return "El código es obligatorio";
        }

        if (string.IsNullOrWhiteSpace(producto.Name))
        {
            return "El nombre es obligatorio";
        }

        if (string.IsNullOrWhiteSpace(producto.UnitOfMeasure))
        {
            return "La unidad de medida es obligatoria";
        }

        if (producto.SalePrice <= 0)
        {
            return "El precio de venta debe ser mayor a 0";
        }

        if (producto.AverageCost < 0)
        {
            return "El costo no puede ser negativo";
        }

        if (producto.TaxRate < 0 || producto.TaxRate > 100)
        {
            return "El impuesto debe estar entre 0 y 100";
        }

        if (producto.Stock < 0)
        {
            return "El stock no puede ser negativo";
        }

        if (producto.MinimumStock < 0)
        {
            return "El stock mínimo no puede ser negativo";
        }

        return null;
    }

    private static void Normalizar(Product producto)
    {
        producto.Code = producto.Code.Trim();
        producto.Name = producto.Name.Trim();
        producto.UnitOfMeasure = producto.UnitOfMeasure.Trim();
        producto.Barcode = string.IsNullOrWhiteSpace(producto.Barcode) ? null : producto.Barcode.Trim();
    }

    // Consultas
    public async Task<List<Product>> ObtenerTodos(bool soloActivos = true)
    {
        var consulta = _db.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (soloActivos)
        {
            consulta = consulta.Where(p => p.IsActive);
        }

        return await consulta
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> ObtenerPorId(int id)
    {
        return await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> Buscar(string texto)
    {
        texto = texto.Trim().ToLower();

        return await _db.Products
            .Where(p => p.IsActive &&
                (p.Name.ToLower().Contains(texto) ||
                 p.Code.ToLower().Contains(texto) ||
                 (p.Barcode != null && p.Barcode == texto)))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<List<Product>> ObtenerStockBajo()
    {
        return await _db.Products
            .Where(p => p.IsActive && p.Stock <= p.MinimumStock)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    // Verificar sin guardar en la base de datos
    public async Task<string> VerificarCreacion(Product producto)
    {
        var error = Validar(producto);
        if (error != null)
        {
            return error;
        }

        Normalizar(producto);

        bool existeCodigo = await _db.Products.AnyAsync(p => p.Code == producto.Code);
        if (existeCodigo)
        {
            return "Ya existe un producto con ese código";
        }

        if (producto.Barcode != null)
        {
            bool existeBarcode = await _db.Products.AnyAsync(p => p.Barcode == producto.Barcode);
            if (existeBarcode)
            {
                return "Ya existe un producto con ese código de barras";
            }
        }

        bool existeCategoria = await _db.Categories.AnyAsync(c => c.Id == producto.CategoryId);
        if (!existeCategoria)
        {
            return "La categoría no existe";
        }

        return "OK";
    }

    public async Task<string> VerificarActualizacion(Product producto)
    {
        var error = Validar(producto);
        if (error != null)
        {
            return error;
        }

        var existente = await _db.Products.FindAsync(producto.Id);
        if (existente == null)
        {
            return "El producto no existe";
        }

        Normalizar(producto);

        bool existeCodigo = await _db.Products.AnyAsync(p => p.Code == producto.Code && p.Id != producto.Id);
        if (existeCodigo)
        {
            return "Ya existe otro producto con ese código";
        }

        if (producto.Barcode != null)
        {
            bool existeBarcode = await _db.Products.AnyAsync(p => p.Barcode == producto.Barcode && p.Id != producto.Id);
            if (existeBarcode)
            {
                return "Ya existe otro producto con ese código de barras";
            }
        }

        bool existeCategoria = await _db.Categories.AnyAsync(c => c.Id == producto.CategoryId);
        if (!existeCategoria)
        {
            return "La categoría no existe";
        }

        return "OK";
    }

    public async Task<string> VerificarCambioEstado(int id)
    {
        bool existe = await _db.Products.AnyAsync(p => p.Id == id);
        if (!existe)
        {
            return "El producto no existe";
        }

        return "OK";
    }
}