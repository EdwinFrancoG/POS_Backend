namespace POS_Backend.Logica.Dtos;

// Datos Entrantes
public record AsientoLineaRequest(
    int LedgerAccountId,
    decimal Debit,
    decimal Credit,
    string? Description);

public record AsientoRequest(
    DateTime Date,
    string? Description,
    int UserId,
    List<AsientoLineaRequest> Lines);

public record AnularAsientoRequest(
    string? Description,
    int UserId);

// Datos Salientes
public record AsientoLineaDto(
    int LedgerAccountId,
    string AccountCode,
    string AccountName,
    decimal Debit,
    decimal Credit,
    string? Description);

public record AsientoDto(
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
    List<AsientoLineaDto> Lines);