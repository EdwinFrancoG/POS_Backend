using Microsoft.Extensions.Logging;
using POS_Backend.Datos.Models;
using POS_Backend.Logica.Constants;
using POS_Backend.Logica.Dtos;
using POS_Backend.Logica.Exceptions;
using POS_Backend.Logica.Services;
using static POS_Backend.Logica.Exceptions.DomainExceptionFilter;

namespace POS_Backend.Logica.Domain;

public class JournalEntryDomain
{
    private const int MinimumLines = 2;
    private const int MoneyDecimals = 2;

    private readonly IJournalEntryService _journalEntryService;
    private readonly ILogger<JournalEntryDomain> _logger;

    public JournalEntryDomain(IJournalEntryService journalEntryService, ILogger<JournalEntryDomain> logger)
    {
        _journalEntryService = journalEntryService;
        _logger = logger;
    }

    public async Task<JournalEntryDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var entry = await _journalEntryService.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"No existe el asiento con id {id}.");

            return ToDto(entry);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al obtener el asiento {JournalEntryId}.", id);
            throw new DomainException($"No se pudo obtener el asiento con id {id}.", ex);
        }
    }

    public async Task<List<JournalEntryDto>> GetAllAsync(
        DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        try
        {
            var entries = await _journalEntryService.GetAllAsync(from?.Date, to?.Date.AddDays(1), ct);
            return entries.Select(ToDto).ToList();
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al listar los asientos.");
            throw new DomainException("No se pudo obtener la lista de asientos.", ex);
        }
    }

    public async Task<JournalEntryDto> CreateAsync(JournalEntryRequest request, CancellationToken ct = default)
    {
        try
        {
            var lines = request.Lines ?? new List<JournalEntryLineRequest>();
            var totalDebit = lines.Sum(l => l.Debit);
            var totalCredit = lines.Sum(l => l.Credit);

            ValidateRequest(request, lines, totalDebit, totalCredit);

            var accountIds = lines.Select(l => l.LedgerAccountId).Distinct().ToList();
            var accounts = await _journalEntryService.GetLedgerAccountsByIdsAsync(accountIds, ct);
            ValidateAccounts(accountIds, accounts);

            var entry = new JournalEntry
            {
                Date = request.Date.Date,
                Description = CleanText(request.Description),
                Source = JournalEntrySource.Manual,
                Status = JournalEntryStatus.Posted,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                UserId = request.UserId,
                JournalEntryLines = lines.Select(l => new JournalEntryLine
                {
                    LedgerAccountId = l.LedgerAccountId,
                    Debit = l.Debit,
                    Credit = l.Credit,
                    Description = CleanText(l.Description)
                }).ToList()
            };

            var created = await _journalEntryService.AddAsync(entry, ct);
            return ToDto(created);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al crear el asiento.");
            throw new DomainException("No se pudo crear el asiento.", ex);
        }
    }

    public async Task<JournalEntryDto> VoidAsync(int id, VoidJournalEntryRequest request, CancellationToken ct = default)
    {
        try
        {
            if (request.UserId <= 0)
                throw new BusinessRuleException("El usuario es obligatorio.");

            var original = await _journalEntryService.GetForVoidAsync(id, ct)
                ?? throw new NotFoundException($"No existe el asiento con id {id}.");

            EnsureCanBeVoided(original);

            var reversal = BuildReversal(original, request);
            original.Status = JournalEntryStatus.Voided;

            var created = await _journalEntryService.TryAddReversalAsync(original, reversal, ct)
                ?? throw new BusinessRuleException("Este asiento ya tiene una reversa registrada.");

            return ToDto(created);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al anular el asiento {JournalEntryId}.", id);
            throw new DomainException($"No se pudo anular el asiento con id {id}.", ex);
        }
    }

    private static void ValidateRequest(
        JournalEntryRequest request, List<JournalEntryLineRequest> lines, decimal totalDebit, decimal totalCredit)
    {
        var errors = new List<string>();

        if (request.UserId <= 0)
            errors.Add("El usuario es obligatorio.");

        if (request.Date.Date > DateTime.Today)
            errors.Add("La fecha del asiento no puede ser futura.");

        if (lines.Count < MinimumLines)
            errors.Add($"El asiento necesita al menos {MinimumLines} líneas.");

        for (var i = 0; i < lines.Count; i++)
            ValidateLine(lines[i], i + 1, errors);

        if (errors.Count == 0 && totalDebit != totalCredit)
            errors.Add($"El asiento no cuadra: débitos {totalDebit:N2} y créditos {totalCredit:N2}.");

        if (errors.Count > 0) throw new BusinessRuleException(errors);
    }

    private static void ValidateLine(JournalEntryLineRequest line, int number, List<string> errors)
    {
        if (line.Debit < 0 || line.Credit < 0)
            errors.Add($"Línea {number}: los montos no pueden ser negativos.");
        else if ((line.Debit > 0) == (line.Credit > 0))
            errors.Add($"Línea {number}: debe tener un débito o un crédito, no ambos ni ninguno.");

        if (decimal.Round(line.Debit, MoneyDecimals) != line.Debit ||
            decimal.Round(line.Credit, MoneyDecimals) != line.Credit)
            errors.Add($"Línea {number}: los montos admiten máximo {MoneyDecimals} decimales.");
    }

    private static void ValidateAccounts(List<int> accountIds, Dictionary<int, LedgerAccount> accounts)
    {
        var errors = new List<string>();

        foreach (var id in accountIds)
        {
            if (!accounts.TryGetValue(id, out var account))
                errors.Add($"La cuenta con id {id} no existe.");
            else if (!account.IsActive)
                errors.Add($"La cuenta {account.Code} - {account.Name} está inactiva.");
            else if (!account.AllowsPosting)
                errors.Add($"La cuenta {account.Code} - {account.Name} no acepta movimientos (es una cuenta de agrupación).");
        }

        if (errors.Count > 0) throw new BusinessRuleException(errors);
    }

    private static void EnsureCanBeVoided(JournalEntry entry)
    {
        if (entry.Status == JournalEntryStatus.Voided)
            throw new BusinessRuleException("El asiento ya está anulado.");

        if (entry.Source == JournalEntrySource.Reversal || entry.ReversedEntryId is not null)
            throw new BusinessRuleException("No se puede anular un asiento de reversa.");

        if (IsGeneratedByAnotherModule(entry))
            throw new BusinessRuleException(
                "Este asiento lo generó otro módulo (venta, compra, pago o inventario) y debe anularse desde ese módulo.");
    }

    private static bool IsGeneratedByAnotherModule(JournalEntry entry) =>
        entry.InvoiceId is not null ||
        entry.PurchaseOrderId is not null ||
        entry.PaymentId is not null ||
        entry.InventoryMovementId is not null ||
        entry.SupplierPaymentId is not null;

    private static JournalEntry BuildReversal(JournalEntry original, VoidJournalEntryRequest request) => new()
    {
        Date = DateTime.Today,
        Description = CleanText(request.Description) ?? $"Anulación del asiento {original.Number}",
        Source = JournalEntrySource.Reversal,
        Status = JournalEntryStatus.Posted,
        ReversedEntryId = original.Id,
        TotalDebit = original.TotalCredit,
        TotalCredit = original.TotalDebit,
        UserId = request.UserId,
        JournalEntryLines = original.JournalEntryLines.Select(l => new JournalEntryLine
        {
            LedgerAccountId = l.LedgerAccountId,
            Debit = l.Credit,
            Credit = l.Debit,
            Description = l.Description
        }).ToList()
    };

    private static string? CleanText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static JournalEntryDto ToDto(JournalEntry entry) => new(
        entry.Id, entry.Number, entry.Date, entry.Description, entry.Source, entry.Status,
        entry.TotalDebit, entry.TotalCredit, entry.UserId, entry.ReversedEntryId,
        entry.JournalEntryLines
            .OrderBy(l => l.Id)
            .Select(l => new JournalEntryLineDto(
                l.LedgerAccountId, l.LedgerAccount.Code, l.LedgerAccount.Name,
                l.Debit, l.Credit, l.Description))
            .ToList());
}