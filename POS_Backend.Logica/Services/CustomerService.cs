using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;

namespace POS_Backend.Logica.Servicios;

public class CustomerService
{
     private readonly AppDbContext _db;
    public CustomerService(AppDbContext db) => _db = db;

    //Consultas
     public async Task<List<Customers>> ObtenerTodosAsync(bool soloActivos = true) =>
        await _db.Customers
            .Where(c => !soloActivos || c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
     public async Task<Customers?> ObtenerPorIdAsync(int id) =>
        await _db.Customers.FindAsync(id);

    public async Task<List<Customers>> BuscarAsync(string texto)
    {
        texto = texto.Trim().ToLower();
        return await _db.Customers
            .Where(c => c.IsActive &&
                (c.Name.ToLower().Contains(texto) ||
                 c.Code.ToLower().Contains(texto) ||
                 (c.Rtn != null && c.Rtn.Contains(texto))))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    //Operaciones
    //Crear
     public async Task<Resultado> CrearAsync(Customers cliente)
    {
        var error = Validar(cliente);
        if (error != null) return new(false, error);

        Normalizar(cliente);

        if (await _db.Customers.AnyAsync(c => c.Code == cliente.Code))
            return new(false, "Ya existe un cliente con ese código");

        if (cliente.Rtn != null &&
            await _db.Customers.AnyAsync(c => c.Rtn == cliente.Rtn))
            return new(false, "Ya existe un cliente con ese RTN");

        cliente.IsActive = true;
        _db.Customers.Add(cliente);
        await _db.SaveChangesAsync();
        return new(true, null);
    }
    //Actualizar
    public async Task<Resultado> ActualizarAsync(Customers cliente)
    {
        var error = Validar(cliente);
        if (error != null) return new(false, error);

        var existente = await _db.Customers.FindAsync(cliente.Id);
        if (existente == null)
            return new(false, "El cliente no existe");

        Normalizar(cliente);

        if (await _db.Customers.AnyAsync(c => c.Code == cliente.Code && c.Id != cliente.Id))
            return new(false, "Ya existe otro cliente con ese código");

        if (cliente.Rtn != null &&
            await _db.Customers.AnyAsync(c => c.Rtn == cliente.Rtn && c.Id != cliente.Id))
            return new(false, "Ya existe otro cliente con ese RTN");

        existente.Code = cliente.Code;
        existente.Name = cliente.Name;
        existente.Rtn = cliente.Rtn;
        existente.Phone = cliente.Phone;
        existente.Email = cliente.Email;
        existente.Address = cliente.Address;
        existente.CreditLimit = cliente.CreditLimit;
        existente.CreditDays = cliente.CreditDays;

        await _db.SaveChangesAsync();
        return new(true, null);
    }
    //Cambiar estado
    public async Task<Resultado> CambiarEstadoAsync(int id, bool activo)
    {
        var cliente = await _db.Customers.FindAsync(id);
        if (cliente == null)
            return new(false, "El cliente no existe");

        cliente.IsActive = activo;
        await _db.SaveChangesAsync();
        return new(true, null);
    }
    //Validaciones
     private static string? Validar(Customers c)
    {
        if (string.IsNullOrWhiteSpace(c.Code)) return "El código es obligatorio";
        if (string.IsNullOrWhiteSpace(c.Name)) return "El nombre es obligatorio";

        if (!string.IsNullOrWhiteSpace(c.Rtn))
        {
            var rtn = c.Rtn.Replace("-", "").Trim();
            if (rtn.Length != 14 || !rtn.All(char.IsDigit))
                return "El RTN debe tener 14 dígitos";
        }

        if (!string.IsNullOrWhiteSpace(c.Email) && !CorreoValido(c.Email))
            return "El correo electrónico no es válido";

        if (!string.IsNullOrWhiteSpace(c.Phone))
        {
            var digitos = c.Phone.Count(char.IsDigit);
            if (digitos < 8)
                return "El teléfono debe tener al menos 8 dígitos";
        }

        if (c.CreditLimit < 0) return "El límite de crédito no puede ser negativo";
        if (c.CreditDays < 0) return "Los días de crédito no pueden ser negativos";
        if (c.CreditLimit == 0 && c.CreditDays > 0)
            return "No se pueden asignar días de crédito sin un límite de crédito";

        return null;
    }
    private static void Normalizar(Customers c)
    {
        c.Code = c.Code.Trim();
        c.Name = c.Name.Trim();
        c.Rtn = string.IsNullOrWhiteSpace(c.Rtn) ? null : c.Rtn.Replace("-", "").Trim();
        c.Email = string.IsNullOrWhiteSpace(c.Email) ? null : c.Email.Trim();
        c.Phone = string.IsNullOrWhiteSpace(c.Phone) ? null : c.Phone.Trim();
    }

    private static bool CorreoValido(string correo)
    {
        try { return new System.Net.Mail.MailAddress(correo.Trim()).Address == correo.Trim(); }
        catch { return false; }
    }
}