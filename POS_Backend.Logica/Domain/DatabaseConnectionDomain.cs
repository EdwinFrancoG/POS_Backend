using Microsoft.Extensions.Logging;
using POS_Backend.Logica.Dtos;
using POS_Backend.Logica.Services;

namespace POS_Backend.Logica.Domain;

public class DatabaseConnectionDomain
{
    private readonly IDatabaseConnectionService _databaseConnectionService;
    private readonly ILogger<DatabaseConnectionDomain> _logger;

    public DatabaseConnectionDomain(
        IDatabaseConnectionService databaseConnectionService,
        ILogger<DatabaseConnectionDomain> logger)
    {
        _databaseConnectionService = databaseConnectionService;
        _logger = logger;
    }

    public async Task<ConnectionStatusDto> CheckAsync(CancellationToken ct = default)
    {
        try
        {
            await _databaseConnectionService.VerifyConnectionAsync(ct);
            return new ConnectionStatusDto(true, null);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "No se pudo conectar con la base de datos.");
            return new ConnectionStatusDto(false, ex.Message);
        }
    }
}