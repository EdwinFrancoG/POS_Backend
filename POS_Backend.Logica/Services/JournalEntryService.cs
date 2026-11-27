using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Logica.Services;

public class JournalEntryService : IJournalEntryService
{
    private const string NumberPrefix = "AS-";
    private const string TemporaryNumberPrefix = "TMP-";

    private readonly AppDbContext _db;

    public JournalEntryService(AppDbContext db) => _db = db;

    public async Task<JournalEntry?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await FullQuery().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<JournalEntry?> GetForVoidAsync(int id, CancellationToken ct = default) =>
        await _db.JournalEntries
            .Include(e => e.JournalEntryLines)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<List<JournalEntry>> GetAllAsync(
        DateTime? fromInclusive, DateTime? toExclusive, CancellationToken ct = default)
    {
        var query = FullQuery().AsNoTracking();

        if (fromInclusive is not null)
            query = query.Where(e => e.Date >= fromInclusive.Value);

        if (toExclusive is not null)
            query = query.Where(e => e.Date < toExclusive.Value);

        return await query
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.Id)
            .ToListAsync(ct);
    }

    public async Task<Dictionary<int, LedgerAccount>> GetLedgerAccountsByIdsAsync(
        List<int> ids, CancellationToken ct = default) =>
        await _db.LedgerAccounts
            .AsNoTracking()
            .Where(a => ids.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct);

    public async Task<JournalEntry> AddAsync(JournalEntry entry, CancellationToken ct = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);

        await AddWithNumberAsync(entry, ct);

        await transaction.CommitAsync(ct);

        return await LoadSavedAsync(entry.Id, ct);
    }

    public async Task<JournalEntry?> TryAddReversalAsync(
        JournalEntry original, JournalEntry reversal, CancellationToken ct = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);

        if (await _db.JournalEntries.AnyAsync(e => e.ReversedEntryId == original.Id, ct))
            return null;

        await AddWithNumberAsync(reversal, ct);

        await transaction.CommitAsync(ct);

        return await LoadSavedAsync(reversal.Id, ct);
    }

    private IQueryable<JournalEntry> FullQuery() =>
        _db.JournalEntries
            .Include(e => e.JournalEntryLines)
            .ThenInclude(l => l.LedgerAccount);

    private async Task AddWithNumberAsync(JournalEntry entry, CancellationToken ct)
    {
        entry.Number = TemporaryNumberPrefix + Guid.NewGuid().ToString("N")[..12];
        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync(ct);

        entry.Number = $"{NumberPrefix}{entry.Id:D6}";
        await _db.SaveChangesAsync(ct);
    }

    private async Task<JournalEntry> LoadSavedAsync(int id, CancellationToken ct) =>
        await GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"No se pudo recuperar el asiento {id} después de guardarlo.");
}