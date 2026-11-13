using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;

namespace POS_Backend.Logica.Servicios;

public class CategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db) => _db = db;

    //Consultas
    public async Task<List<Category>> ObtenerTodasAsync(bool soloActivas = true) =>
    await _db.Category
    .Where(c => !soloActivas || c.IsActive)
    .OrderBy(c => c.Name)
    .ToListAsync();

     public async Task<Category?> ObtenerPorIdAsync(int id) =>
        await _db.Category.FindAsync(id);
    
    // Operaciones
    //Crear
    public async Task<Resultado> CrearAsync(Category categoria)
    {
        var error = Validar(categoria);
        if (error != null) return new(false, error);

        categoria.Name = categoria.Name.Trim();

        if (await _db.Category.AnyAsync(c => c.Name.ToLower() == categoria.Name.ToLower()))
            return new(false, "Ya existe una categoría con ese nombre");

        categoria.IsActive = true;
        _db.Category.Add(categoria);
        await _db.SaveChangesAsync();
        return new(true, null);
    }
    //Actualizar
    public async Task<Resultado> ActualizarAsync(Category categoria)
    {
        var error = Validar(categoria);
        if (error != null) return new(false, error);

        var existente = await _db.Category.FindAsync(categoria.Id);
        if (existente == null)
            return new(false, "La categoría no existe");

        categoria.Name = categoria.Name.Trim();

        if (await _db.Category.AnyAsync(c =>
                c.Name.ToLower() == categoria.Name.ToLower() && c.Id != categoria.Id))
            return new(false, "Ya existe otra categoría con ese nombre");

        existente.Name = categoria.Name;
        existente.Description = categoria.Description;

        await _db.SaveChangesAsync();
        return new(true, null);
    }
    //Cambiar estado
        public async Task<Resultado> CambiarEstadoAsync(int id, bool activa)
    {
        var categoria = await _db.Category.FindAsync(id);
        if (categoria == null)
            return new(false, "La categoría no existe");

        if (!activa && await _db.Product.AnyAsync(p => p.CategoryId == id && p.IsActive))
            return new(false, "No se puede desactivar: tiene productos activos");

        categoria.IsActive = activa;
        await _db.SaveChangesAsync();
        return new(true, null);
    }
    //Las validaciones
    private static string? Validar(Category c)
    {
        if (string.IsNullOrWhiteSpace(c.Name)) return "El nombre es obligatorio";
        if (c.Name.Trim().Length < 2) return "El nombre es demasiado corto";
        return null;
    }
}