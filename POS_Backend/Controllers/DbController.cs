using Microsoft.AspNetCore.Mvc;
using POS_Backend.Logica.Servicios;

namespace POS_Backend.Controllers;

[ApiController]
[Route("db")]
public class DbController : ControllerBase
{
    private readonly ConexionService _conexion;

    public DbController(ConexionService conexion) => _conexion = conexion;

    [HttpGet("estado")]
    public async Task<IActionResult> Estado(CancellationToken ct)
    {
        var estado = await _conexion.ProbarAsync(ct);
        return estado.Conectado ? Ok(estado) : StatusCode(503, estado);
    }
}