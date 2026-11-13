using Microsoft.AspNetCore.Mvc;
using POS_Backend.Logica.Dtos;
using POS_Backend.Logica.Services;

namespace POS_Backend.Controllers;

[ApiController]
[Route("api/asientos")]
public class AsientosController : ControllerBase
{
    private readonly AsientoService _servicio;

    public AsientosController(AsientoService servicio) => _servicio = servicio;

    [HttpGet]
    public async Task<ActionResult<List<AsientoDto>>> Listar(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken ct)
        => Ok(await _servicio.ListarAsync(desde, hasta, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AsientoDto>> Obtener(int id, CancellationToken ct)
        => Ok(await _servicio.ObtenerAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<AsientoDto>> Crear(AsientoRequest req, CancellationToken ct)
    {
        var creado = await _servicio.CrearAsync(req, ct);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [HttpPost("{id:int}/anular")]
    public async Task<ActionResult<AsientoDto>> Anular(int id, AnularAsientoRequest req, CancellationToken ct)
    {
        var reversa = await _servicio.AnularAsync(id, req, ct);
        return CreatedAtAction(nameof(Obtener), new { id = reversa.Id }, reversa);
    }
}