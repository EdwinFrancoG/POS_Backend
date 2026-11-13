using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;
using POS_Backend.Logica.Dtos;
using POS_Backend.Logica.Exceptions;

namespace POS_Backend.Logica.Services;

public class ProveedorService
{
    private readonly AppDbContext _db;

    public ProveedorService(AppDbContext db) => _db = db;

    // Querys
    public async Task<List<ProveedorDto>> ListarAsync(bool soloActivos = true, CancellationToken ct = default)
    {
        var query = _db.Suppliers.AsNoTracking();
        if (soloActivos) query = query.Where(s => s.IsActive);

        var lista = await query.OrderBy(s => s.Name).ToListAsync(ct);
        return lista.Select(ToDto).ToList();
    }

    public async Task<ProveedorDto> ObtenerAsync(int id, CancellationToken ct = default)
    {
        var proveedor = await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NoEncontradoException($"No existe el proveedor con id {id}.");
        return ToDto(proveedor);
    }

    // C/U Crear y Actualizar

    public async Task<ProveedorDto> CrearAsync(ProveedorRequest req, CancellationToken ct = default)
    {
        var datos = Normalizar(req);
        await ValidarAsync(datos, null, ct);

        var proveedor = new Supplier
        {
            Code = datos.Code,
            Name = datos.Name,
            Rtn = datos.Rtn,
            ContactName = datos.ContactName,
            Phone = datos.Phone,
            Email = datos.Email,
            Address = datos.Address,
            CreditDays = datos.CreditDays,
            IsActive = true
        };

        _db.Suppliers.Add(proveedor);
        await _db.SaveChangesAsync(ct);
        return ToDto(proveedor);
    }

    public async Task<ProveedorDto> EditarAsync(int id, ProveedorRequest req, CancellationToken ct = default)
    {
        var proveedor = await _db.Suppliers.FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NoEncontradoException($"No existe el proveedor con id {id}.");

        var datos = Normalizar(req);
        await ValidarAsync(datos, id, ct);

        proveedor.Code = datos.Code;
        proveedor.Name = datos.Name;
        proveedor.Rtn = datos.Rtn;
        proveedor.ContactName = datos.ContactName;
        proveedor.Phone = datos.Phone;
        proveedor.Email = datos.Email;
        proveedor.Address = datos.Address;
        proveedor.CreditDays = datos.CreditDays;

        await _db.SaveChangesAsync(ct);
        return ToDto(proveedor);
    }

    // Enable & Disable
    public Task DesactivarAsync(int id, CancellationToken ct = default) => CambiarEstadoAsync(id, false, ct);

    public Task ActivarAsync(int id, CancellationToken ct = default) => CambiarEstadoAsync(id, true, ct);

    private async Task CambiarEstadoAsync(int id, bool activo, CancellationToken ct)
    {
        var proveedor = await _db.Suppliers.FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NoEncontradoException($"No existe el proveedor con id {id}.");

        if (proveedor.IsActive == activo) return;

        // Al desactivar valida que no tenga órdenes, ni saldo pendiente

        proveedor.IsActive = activo;
        await _db.SaveChangesAsync(ct);
    }

    // Validations
    private async Task ValidarAsync(ProveedorRequest d, int? idActual, CancellationToken ct)
    {
        var errores = new List<string>();

        if (d.Code.Length == 0) errores.Add("El código es obligatorio.");
        else if (d.Code.Length > 20) errores.Add("El código no puede pasar de 20 caracteres.");

        if (d.Name.Length == 0) errores.Add("El nombre es obligatorio.");
        else if (d.Name.Length > 150) errores.Add("El nombre no puede pasar de 150 caracteres.");

        if (d.Rtn is not null && d.Rtn.Length != 14) errores.Add("El RTN debe tener 14 dígitos.");

        if (d.Email is not null && !EmailValido(d.Email)) errores.Add("El correo no tiene un formato válido.");

        if (d.CreditDays < 0 || d.CreditDays > 365) errores.Add("Los días de crédito deben estar entre 0 y 365.");
        if (errores.Count > 0) throw new ReglaNegocioException(errores);

        var id = idActual ?? 0;

        if (await _db.Suppliers.AnyAsync(s => s.Code == d.Code && s.Id != id, ct))
            errores.Add($"Ya existe un proveedor con el código nº: '{d.Code}'.");

        if (d.Rtn is not null && await _db.Suppliers.AnyAsync(s => s.Rtn == d.Rtn && s.Id != id, ct))
            errores.Add("Ya existe un proveedor registrado con ese RTN.");

        if (errores.Count > 0) throw new ReglaNegocioException(errores);
    }

    // Data Clean

    private static ProveedorRequest Normalizar(ProveedorRequest r) => r with
    {
        Code = (r.Code ?? "").Trim().ToUpperInvariant(),
        Name = (r.Name ?? "").Trim(),
        Rtn = string.IsNullOrWhiteSpace(r.Rtn) ? null : new string(r.Rtn.Where(char.IsDigit).ToArray()),
        ContactName = Limpiar(r.ContactName),
        Phone = Limpiar(r.Phone),
        Email = Limpiar(r.Email)?.ToLowerInvariant(),
        Address = Limpiar(r.Address)
    };

    private static string? Limpiar(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static bool EmailValido(string email)
    {
        try { return new MailAddress(email).Address == email; }
        catch (FormatException) { return false; }
    }

    private static ProveedorDto ToDto(Supplier s) =>
        new(s.Id, s.Code, s.Name, s.Rtn, s.ContactName, s.Phone, s.Email, s.Address, s.CreditDays, s.IsActive);
}