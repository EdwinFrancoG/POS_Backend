namespace POS_Backend.Logica.Dtos;

public record JournalEntryLineRequest(
    int LedgerAccountId,
    decimal Debit,
    decimal Credit,
    string? Description);

public record JournalEntryRequest(
    DateTime Date,
    string? Description,
    int UserId,
    List<JournalEntryLineRequest> Lines);

public record VoidJournalEntryRequest(
    string? Description,
    int UserId);

public record JournalEntryLineDto(
    int LedgerAccountId,
    string AccountCode,
    string AccountName,
    decimal Debit,
    decimal Credit,
    string? Description);

public record JournalEntryDto(
    int Id,
    string Number,
    DateTime Date,
    string? Description,
    string Source,
    string Status,
    decimal TotalDebit,
    decimal TotalCredit,
    int UserId,
    int? ReversedEntryId,
    List<JournalEntryLineDto> Lines);