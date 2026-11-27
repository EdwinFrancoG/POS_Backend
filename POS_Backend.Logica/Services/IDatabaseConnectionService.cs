namespace POS_Backend.Logica.Services;

public interface IDatabaseConnectionService
{
    Task VerifyConnectionAsync(CancellationToken ct = default);
}
