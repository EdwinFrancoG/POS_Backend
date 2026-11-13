using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;
using POS_Backend.Logica.Constants;
using POS_Backend.Logica.Dtos;
using POS_Backend.Logica.Exceptions;

namespace POS_Backend.Logica.Services;

public class AsientoService
{
    private readonly AppDbContext _db;

    public AsientoService(AppDbContext db) => _db = db;

    // Querys

    public async Task<AsientoDto> ObtenerAsync(int id, CancellationToken ct = default)
    {
        var asiento = await ConsultaCompleta().AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NoEncontradoException($"No existe el asiento con id {id}.");
        return ToDto(asiento);
    }

    public async Task<List<AsientoDto>> ListarAsync(
        DateTime? desde = null, DateTime? hasta = null, CancellationToken ct = default)
    {
        var query = ConsultaCompleta().AsNoTracking();

        if (desde is not null) query = query.Where(a => a.Date >= desde.Value.Date);
        if (hasta is not null) query = query.Where(a => a.Date < hasta.Value.Date.AddDays(1));

        var lista = await query
            .OrderByDescending(a => a.Date)
            .ThenByDescending(a => a.Id)
            .ToListAsync(ct);

        return lista.Select(ToDto).ToList();
    }

    private IQueryable<JournalEntry> ConsultaCompleta() =>
        _db.JournalEntries
            .Include(a => a.JournalEntryLines)
            .ThenInclude(l => l.LedgerAccount);

    // Create

    public async Task<AsientoDto> CrearAsync(AsientoRequest req, CancellationToken ct = default)
    {
        var errores = new List<string>();

        if (req.UserId <= 0) errores.Add("El usuario es obligatorio.");
        if (req.Date.Date > DateTime.Today) errores.Add("La fecha del asiento no puede ser futura.");

        var lineas = req.Lines ?? new List<AsientoLineaRequest>();
        if (lineas.Count < 2) errores.Add("El asiento necesita al menos 2 líneas.");

        for (var i = 0; i < lineas.Count; i++)
        {
            var l = lineas[i];
            var n = i + 1;

            if (l.Debit < 0 || l.Credit < 0)
                errores.Add($"Línea {n}: los montos no pueden ser negativos.");
            else if ((l.Debit > 0) == (l.Credit > 0))
                errores.Add($"Línea {n}: debe tener un débito o un crédito, no ambos ni ninguno.");

            if (decimal.Round(l.Debit, 2) != l.Debit || decimal.Round(l.Credit, 2) != l.Credit)
                errores.Add($"Línea {n}: los montos admiten máximo 2 decimales.");
        }

        var totalDebito = lineas.Sum(l => l.Debit);
        var totalCredito = lineas.Sum(l => l.Credit);

        if (errores.Count == 0 && totalDebito != totalCredito)
            errores.Add($"El asiento no cuadra: débitos {totalDebito:N2} y créditos {totalCredito:N2}.");

        if (errores.Count > 0) throw new ReglaNegocioException(errores);

        var ids = lineas.Select(l => l.LedgerAccountId).Distinct().ToList();
        var cuentas = await _db.LedgerAccounts
            .Where(c => ids.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, ct);

        foreach (var id in ids)
        {
            if (!cuentas.TryGetValue(id, out var cuenta))
                errores.Add($"La cuenta con id {id} no existe.");
            else if (!cuenta.IsActive)
                errores.Add($"La cuenta {cuenta.Code} - {cuenta.Name} está inactiva.");
            else if (!cuenta.AllowsPosting)
                errores.Add($"La cuenta {cuenta.Code} - {cuenta.Name} no acepta movimientos (es una cuenta de agrupación).");
        }

        if (errores.Count > 0) throw new ReglaNegocioException(errores);

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var asiento = new JournalEntry
        {
            Number = "TMP-" + Guid.NewGuid().ToString("N")[..12],
            Date = req.Date.Date,
            Description = Limpiar(req.Description),
            Source = OrigenAsiento.Manual,
            Status = EstadoAsiento.Contabilizado,
            TotalDebit = totalDebito,
            TotalCredit = totalCredito,
            UserId = req.UserId,
            JournalEntryLines = lineas.Select(l => new JournalEntryLine
            {
                LedgerAccountId = l.LedgerAccountId,
                Debit = l.Debit,
                Credit = l.Credit,
                Description = Limpiar(l.Description)
            }).ToList()
        };

        _db.JournalEntries.Add(asiento);
        await _db.SaveChangesAsync(ct);          

        asiento.Number = $"AS-{asiento.Id:D6}";  
        await _db.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);

        return await ObtenerAsync(asiento.Id, ct);
    }

    // Invalidate

    public async Task<AsientoDto> AnularAsync(int id, AnularAsientoRequest req, CancellationToken ct = default)
    {
        if (req.UserId <= 0) throw new ReglaNegocioException("El usuario es obligatorio.");

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var original = await _db.JournalEntries
            .Include(a => a.JournalEntryLines)
            .FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NoEncontradoException($"No existe el asiento con id {id}.");

        if (original.Status == EstadoAsiento.Anulado)
            throw new ReglaNegocioException("El asiento ya está anulado.");

        if (original.Source == OrigenAsiento.Reversa || original.ReversedEntryId is not null)
            throw new ReglaNegocioException("No se puede anular un asiento de reversa.");

        if (original.InvoiceId is not null || original.PurchaseOrderId is not null ||
            original.PaymentId is not null || original.InventoryMovementId is not null ||
            original.SupplierPaymentId is not null)
            throw new ReglaNegocioException(
                "Este asiento lo generó otro módulo (venta, compra, pago o inventario) y debe anularse desde ese módulo.");

        if (await _db.JournalEntries.AnyAsync(a => a.ReversedEntryId == id, ct))
            throw new ReglaNegocioException("Este asiento ya tiene una reversa registrada.");

        var reversa = new JournalEntry
        {
            Number = "TMP-" + Guid.NewGuid().ToString("N")[..12],
            Date = DateTime.Today,
            Description = Limpiar(req.Description) ?? $"Anulación del asiento {original.Number}",
            Source = OrigenAsiento.Reversa,
            Status = EstadoAsiento.Contabilizado,
            ReversedEntryId = original.Id,
            TotalDebit = original.TotalCredit,
            TotalCredit = original.TotalDebit,
            UserId = req.UserId,
            JournalEntryLines = original.JournalEntryLines.Select(l => new JournalEntryLine
            {
                LedgerAccountId = l.LedgerAccountId,
                Debit = l.Credit,      
                Credit = l.Debit,     
                Description = l.Description
            }).ToList()
        };

        original.Status = EstadoAsiento.Anulado;

        _db.JournalEntries.Add(reversa);
        await _db.SaveChangesAsync(ct);

        reversa.Number = $"AS-{reversa.Id:D6}";
        await _db.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);

        return await ObtenerAsync(reversa.Id, ct);
    }

    // Clean
    private static string? Limpiar(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static AsientoDto ToDto(JournalEntry a) => new(
        a.Id, a.Number, a.Date, a.Description, a.Source, a.Status,
        a.TotalDebit, a.TotalCredit, a.UserId, a.ReversedEntryId,
        a.JournalEntryLines
            .OrderBy(l => l.Id)
            .Select(l => new AsientoLineaDto(
                l.LedgerAccountId, l.LedgerAccount.Code, l.LedgerAccount.Name,
                l.Debit, l.Credit, l.Description))
            .ToList());
}