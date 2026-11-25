using System.Net.Mail;
using Microsoft.Extensions.Logging;
using static POS_Backend.Logica.Exceptions.DomainExceptionFilter;
using POS_Backend.Datos.Models;
using POS_Backend.Logica.Dtos;
using POS_Backend.Logica.Exceptions;
using POS_Backend.Logica.Services;

namespace POS_Backend.Logica.Domain;

public class SupplierDomain
{
    private const int CodeMaxLength = 20;
    private const int NameMaxLength = 150;
    private const int RtnLength = 14;
    private const int CreditDaysMin = 0;
    private const int CreditDaysMax = 365;

    private readonly ISupplierService _supplierService;
    private readonly ILogger<SupplierDomain> _logger;

    public SupplierDomain(ISupplierService supplierService, ILogger<SupplierDomain> logger)
    {
        _supplierService = supplierService;
        _logger = logger;
    }

    public async Task<List<SupplierDto>> GetAllAsync(bool onlyActive = true, CancellationToken ct = default)
    {
        try
        {
            var suppliers = await _supplierService.GetAllAsync(onlyActive, ct);
            return suppliers.Select(ToDto).ToList();
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al listar los proveedores.");
            throw new DomainException("No se pudo obtener la lista de proveedores.", ex);
        }
    }

    public async Task<SupplierDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var supplier = await GetExistingAsync(id, ct);
            return ToDto(supplier);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al obtener el proveedor {SupplierId}.", id);
            throw new DomainException($"No se pudo obtener el proveedor con id {id}.", ex);
        }
    }

    public async Task<SupplierDto> CreateAsync(SupplierRequest request, CancellationToken ct = default)
    {
        try
        {
            var data = Normalize(request);
            await ValidateAsync(data, null, ct);

            var supplier = new Supplier
            {
                Code = data.Code,
                Name = data.Name,
                Rtn = data.Rtn,
                ContactName = data.ContactName,
                Phone = data.Phone,
                Email = data.Email,
                Address = data.Address,
                CreditDays = data.CreditDays,
                IsActive = true
            };

            var created = await _supplierService.AddAsync(supplier, ct);
            return ToDto(created);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al crear el proveedor.");
            throw new DomainException("No se pudo crear el proveedor.", ex);
        }
    }

    public async Task<SupplierDto> UpdateAsync(int id, SupplierRequest request, CancellationToken ct = default)
    {
        try
        {
            var supplier = await GetExistingAsync(id, ct);

            var data = Normalize(request);
            await ValidateAsync(data, id, ct);

            supplier.Code = data.Code;
            supplier.Name = data.Name;
            supplier.Rtn = data.Rtn;
            supplier.ContactName = data.ContactName;
            supplier.Phone = data.Phone;
            supplier.Email = data.Email;
            supplier.Address = data.Address;
            supplier.CreditDays = data.CreditDays;

            await _supplierService.UpdateAsync(supplier, ct);
            return ToDto(supplier);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al actualizar el proveedor {SupplierId}.", id);
            throw new DomainException($"No se pudo actualizar el proveedor con id {id}.", ex);
        }
    }

    public Task DeactivateAsync(int id, CancellationToken ct = default) => ChangeStatusAsync(id, false, ct);

    public Task ActivateAsync(int id, CancellationToken ct = default) => ChangeStatusAsync(id, true, ct);

    private async Task ChangeStatusAsync(int id, bool active, CancellationToken ct)
    {
        try
        {
            var supplier = await GetExistingAsync(id, ct);

            if (supplier.IsActive == active) return;

            supplier.IsActive = active;
            await _supplierService.UpdateAsync(supplier, ct);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            var action = active ? "activar" : "desactivar";
            _logger.LogError(ex, "Error al {Action} el proveedor {SupplierId}.", action, id);
            throw new DomainException($"No se pudo {action} el proveedor con id {id}.", ex);
        }
    }

    private async Task<Supplier> GetExistingAsync(int id, CancellationToken ct) =>
        await _supplierService.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"No existe el proveedor con id {id}.");

    private async Task ValidateAsync(SupplierRequest data, int? currentId, CancellationToken ct)
    {
        var errors = new List<string>();

        if (data.Code.Length == 0) errors.Add("El código es obligatorio.");
        else if (data.Code.Length > CodeMaxLength) errors.Add($"El código no puede pasar de {CodeMaxLength} caracteres.");

        if (data.Name.Length == 0) errors.Add("El nombre es obligatorio.");
        else if (data.Name.Length > NameMaxLength) errors.Add($"El nombre no puede pasar de {NameMaxLength} caracteres.");

        if (data.Rtn is not null && data.Rtn.Length != RtnLength) errors.Add($"El RTN debe tener {RtnLength} dígitos.");

        if (data.Email is not null && !IsValidEmail(data.Email)) errors.Add("El correo no tiene un formato válido.");

        if (data.CreditDays < CreditDaysMin || data.CreditDays > CreditDaysMax)
            errors.Add($"Los días de crédito deben estar entre {CreditDaysMin} y {CreditDaysMax}.");

        if (errors.Count > 0) throw new BusinessRuleException(errors);

        if (await _supplierService.ExistsByCodeAsync(data.Code, currentId, ct))
            errors.Add($"Ya existe un proveedor con el código nº: '{data.Code}'.");

        if (data.Rtn is not null && await _supplierService.ExistsByRtnAsync(data.Rtn, currentId, ct))
            errors.Add("Ya existe un proveedor registrado con ese RTN.");

        if (errors.Count > 0) throw new BusinessRuleException(errors);
    }

    private static SupplierRequest Normalize(SupplierRequest request) => request with
    {
        Code = (request.Code ?? string.Empty).Trim().ToUpperInvariant(),
        Name = (request.Name ?? string.Empty).Trim(),
        Rtn = string.IsNullOrWhiteSpace(request.Rtn) ? null : new string(request.Rtn.Where(char.IsDigit).ToArray()),
        ContactName = CleanText(request.ContactName),
        Phone = CleanText(request.Phone),
        Email = CleanText(request.Email)?.ToLowerInvariant(),
        Address = CleanText(request.Address)
    };

    private static string? CleanText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsValidEmail(string email)
    {
        try { return new MailAddress(email).Address == email; }
        catch (FormatException) { return false; }
    }

    private static SupplierDto ToDto(Supplier s) =>
        new(s.Id, s.Code, s.Name, s.Rtn, s.ContactName, s.Phone, s.Email, s.Address, s.CreditDays, s.IsActive);
}