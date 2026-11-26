using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;

namespace POS_Backend.Logica.Domain;

public class CategoryDomain
{
    private readonly AppDbContext _db;

    public CategoryDomain(AppDbContext db) => _db = db;

    //Consultas
    public async Task<IEnumerable<Category>> GetCategories() =>
        await _db.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();


    public async Task<List<Category>> ObtenerTodasAsync(bool soloActivas = true) =>
    await _db.Categories
    .Where(c => !soloActivas || c.IsActive)
    .OrderBy(c => c.Name)
    .ToListAsync();

    public async Task<Category?> ObtenerPorIdAsync(int id) =>
       await _db.Categories.FindAsync(id);

    // Operaciones
    //Crear
    public async Task<Resultado> CrearAsync(Category categoria)
    {
        var error = Validar(categoria);
        if (error != null) return new(false, error);

        categoria.Name = categoria.Name.Trim();

        if (await _db.Categories.AnyAsync(c => c.Name.ToLower() == categoria.Name.ToLower()))
            return new(false, "Ya existe una categoría con ese nombre");

        categoria.IsActive = true;
        _db.Categories.Add(categoria);
        await _db.SaveChangesAsync();
        return new(true, null);
    }

    public string categoryEvaluate(Category category)
    {
        if (category.Name == null)
        {
            return "El nombre de la categoria esta vacio";
        }
        return "OK";
    }


    //Actualizar
    public async Task<Resultado> ActualizarAsync(Category categoria)
    {
        var error = Validar(categoria);
        if (error != null) return new(false, error);

        var existente = await _db.Categories.FindAsync(categoria.Id);
        if (existente == null)
            return new(false, "La categoría no existe");

        categoria.Name = categoria.Name.Trim();

        if (await _db.Categories.AnyAsync(c =>
                c.Name.ToLower() == categoria.Name.ToLower() && c.Id != categoria.Id))
            return new(false, "Ya existe otra categoría con ese nombre");

        existente.Name = categoria.Name;
        existente.Description = categoria.Description;

        await _db.SaveChangesAsync();
        return new(true, null);
    }

    // Validaciones internas
    private static string? Validar(Category categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria.Name))
        {
            return "El nombre es obligatorio";
        }

        if (categoria.Name.Trim().Length < 2)
        {
            return "El nombre es demasiado corto";
        }

        return null;
    }

    private static void Normalizar(Category categoria)
    {
        categoria.Name = categoria.Name.Trim();
    }

    // Consultas
    public async Task<List<Category>> ObtenerTodas(bool soloActivas = true)
    {
        var consulta = _db.Categories.AsQueryable();

        if (soloActivas)
        {
            consulta = consulta.Where(c => c.IsActive);
        }

        return await consulta
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> ObtenerPorId(int id)
    {
        return await _db.Categories.FindAsync(id);
    }

    // Verificar sin guardar en la base de datos
    public async Task<string> VerificarCreacion(Category categoria)
    {
        var error = Validar(categoria);
        if (error != null)
        {
            return error;
        }

        Normalizar(categoria);

        bool existeNombre = await _db.Categories.AnyAsync(c => c.Name.ToLower() == categoria.Name.ToLower());
        if (existeNombre)
        {
            return "Ya existe una categoría con ese nombre";
        }

        return "OK";
    }

    public async Task<string> VerificarActualizacion(Category categoria)

    {
        var error = Validar(categoria);
        if (error != null)
        {
            return error;
        }

        var existente = await _db.Categories.FindAsync(categoria.Id);
        if (existente == null)
        {
            return "La categoría no existe";
        }

        Normalizar(categoria);

        bool existeNombre = await _db.Categories.AnyAsync(c =>
            c.Name.ToLower() == categoria.Name.ToLower() && c.Id != categoria.Id);
        if (existeNombre)
        {
            return "Ya existe otra categoría con ese nombre";
        }

        return "OK";
    }
    //Cambiar estado
    public async Task<Resultado> CambiarEstadoAsync(int id, bool activa)
    {
        var categoria = await _db.Categories.FindAsync(id);
        if (categoria == null)
            return new(false, "La categoría no existe");

        if (!activa && await _db.Products.AnyAsync(p => p.CategoryId == id && p.IsActive))
            return new(false, "No se puede desactivar: tiene productos activos");

        categoria.IsActive = activa;
        await _db.SaveChangesAsync();
        return new(true, null);
    }

    public async Task<string> VerificarCambioEstado(int id, bool activa)
    {
        var existe = await _db.Categories.AnyAsync(c => c.Id == id);
        if (!existe)
        {
            return "La categoría no existe";
        }

        if (!activa)
        {
            bool tieneProductosActivos = await _db.Products.AnyAsync(p => p.CategoryId == id && p.IsActive);
            if (tieneProductosActivos)
            {
                return "No se puede desactivar: tiene productos activos";
            }
        }

        return "OK";
    }
}