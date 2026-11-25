using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Logica.Domain;

public class CustomerDomain
{
    private readonly AppDbContext _db;

    public CustomerDomain(AppDbContext db)
    {
        _db = db;
    }

    // Validaciones internas
    private static string? Validar(Customers cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Code))
        {
            return "El código es obligatorio";
        }

        if (string.IsNullOrWhiteSpace(cliente.Name))
        {
            return "El nombre es obligatorio";
        }

        if (!string.IsNullOrWhiteSpace(cliente.Rtn))
        {
            var rtn = cliente.Rtn.Replace("-", "").Trim();
            if (rtn.Length != 14 || !rtn.All(char.IsDigit))
            {
                return "El RTN debe tener 14 dígitos";
            }
        }

        if (!string.IsNullOrWhiteSpace(cliente.Email) && !CorreoValido(cliente.Email))
        {
            return "El correo electrónico no es válido";
        }

        if (!string.IsNullOrWhiteSpace(cliente.Phone))
        {
            var digitos = cliente.Phone.Count(char.IsDigit);
            if (digitos < 8)
            {
                return "El teléfono debe tener al menos 8 dígitos";
            }
        }

        if (cliente.CreditLimit < 0)
        {
            return "El límite de crédito no puede ser negativo";
        }

        if (cliente.CreditDays < 0)
        {
            return "Los días de crédito no pueden ser negativos";
        }

        if (cliente.CreditLimit == 0 && cliente.CreditDays > 0)
        {
            return "No se pueden asignar días de crédito sin un límite de crédito";
        }

        return null;
    }

    private static void Normalizar(Customers cliente)
    {
        cliente.Code = cliente.Code.Trim();
        cliente.Name = cliente.Name.Trim();
        cliente.Rtn = string.IsNullOrWhiteSpace(cliente.Rtn) ? null : cliente.Rtn.Replace("-", "").Trim();
        cliente.Email = string.IsNullOrWhiteSpace(cliente.Email) ? null : cliente.Email.Trim();
        cliente.Phone = string.IsNullOrWhiteSpace(cliente.Phone) ? null : cliente.Phone.Trim();
    }

    private static bool CorreoValido(string correo)
    {
        try
        {
            var email = new System.Net.Mail.MailAddress(correo.Trim());
            return email.Address == correo.Trim();
        }
        catch
        {
            return false;
        }
    }

    // Consultas
    public async Task<List<Customers>> ObtenerTodos(bool soloActivos = true)
    {
        var consulta = _db.Customers.AsQueryable();

        if (soloActivos)
        {
            consulta = consulta.Where(c => c.IsActive);
        }

        return await consulta
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Customers?> ObtenerPorId(int id)
    {
        return await _db.Customers.FindAsync(id);
    }

    public async Task<List<Customers>> Buscar(string texto)
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

    // Verificar sin guardar en la base de datos
    // Verificar la creación
    public async Task<string> VerificarCreacion(Customers cliente)
    {
        var error = Validar(cliente);
        if (error != null)
        {
            return error;
        }

        Normalizar(cliente);

        bool existeCodigo = await _db.Customers.AnyAsync(c => c.Code == cliente.Code);
        if (existeCodigo)
        {
            return "Ya existe un cliente con ese código";
        }

        if (cliente.Rtn != null)
        {
            bool existeRtn = await _db.Customers.AnyAsync(c => c.Rtn == cliente.Rtn);
            if (existeRtn)
            {
                return "Ya existe un cliente con ese RTN";
            }
        }

        return "OK";
    }

    // Verificar la actualización
    public async Task<string> VerificarActualizacion(Customers cliente)
    {
        var error = Validar(cliente);
        if (error != null)
        {
            return error;
        }

        var existente = await _db.Customers.FindAsync(cliente.Id);
        if (existente == null)
        {
            return "El cliente no existe";
        }

        Normalizar(cliente);

        bool existeCodigo = await _db.Customers.AnyAsync(c => c.Code == cliente.Code && c.Id != cliente.Id);
        if (existeCodigo)
        {
            return "Ya existe otro cliente con ese código";
        }

        if (cliente.Rtn != null)
        {
            bool existeRtn = await _db.Customers.AnyAsync(c => c.Rtn == cliente.Rtn && c.Id != cliente.Id);
            if (existeRtn)
            {
                return "Ya existe otro cliente con ese RTN";
            }
        }

        return "OK";
    }

    // Verificar el cambio de estado
    public async Task<string> VerificarCambioEstado(int id)
    {
        bool existe = await _db.Customers.AnyAsync(c => c.Id == id);
        if (!existe)
        {
            return "El cliente no existe";
        }

        return "OK";
    }
}