using Microsoft.AspNetCore.Mvc;
using POS_Backend.Logica.Domain;
using POS_Backend.Logica.Dtos;

namespace POS_Backend.Controllers;

[ApiController]
[Route("db")]
public class DatabaseController : ControllerBase
{
    private readonly DatabaseConnectionDomain _databaseConnectionDomain;

    public DatabaseController(DatabaseConnectionDomain databaseConnectionDomain) =>
        _databaseConnectionDomain = databaseConnectionDomain;

    [HttpGet("estado")]
    public async Task<ActionResult<ConnectionStatusDto>> GetStatus(CancellationToken ct)
    {
        var status = await _databaseConnectionDomain.CheckAsync(ct);

        return status.Connected
            ? Ok(status)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, status);
    }
}