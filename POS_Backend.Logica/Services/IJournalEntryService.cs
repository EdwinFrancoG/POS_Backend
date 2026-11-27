using POS_Backend.Datos.Models;

namespace POS_Backend.Logica.Services;

public interface IJournalEntryService
{
    Task<JournalEntry?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<JournalEntry?> GetForVoidAsync(int id, CancellationToken ct = default);
    Task<List<JournalEntry>> GetAllAsync(DateTime? fromInclusive, DateTime? toExclusive, CancellationToken ct = default);
    Task<Dictionary<int, LedgerAccount>> GetLedgerAccountsByIdsAsync(List<int> ids, CancellationToken ct = default);
    Task<JournalEntry> AddAsync(JournalEntry entry, CancellationToken ct = default);
    Task<JournalEntry?> TryAddReversalAsync(JournalEntry original, JournalEntry reversal, CancellationToken ct = default);
}