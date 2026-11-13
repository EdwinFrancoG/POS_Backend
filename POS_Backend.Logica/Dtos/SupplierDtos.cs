namespace POS_Backend.Logica.Dtos;

public record ProveedorRequest(
    string Code,
    string Name,
    string? Rtn,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Address,
    int CreditDays);

public record ProveedorDto(
    int Id,
    string Code,
    string Name,
    string? Rtn,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Address,
    int CreditDays,
    bool IsActive);