using Microsoft.AspNetCore.Mvc;
using POS_Backend.Logica.Dtos;
using POS_Backend.Logica.Domain;

namespace POS_Backend.Controllers;

[ApiController]
[Route("api/proveedores")]
public class ProveedoresController : ControllerBase
{
    private readonly ProveedorService _servicio;

    public ProveedoresController(ProveedorService servicio) => _servicio = servicio;

    [HttpGet]
    public async Task<ActionResult<List<ProveedorDto>>> getSupplier(
        [FromQuery] bool soloActivos = true, CancellationToken ct = default)
        => Ok(await _servicio.ListarAsync(soloActivos, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProveedorDto>> getSupplierById(int id, CancellationToken ct)
        => Ok(await _servicio.ObtenerAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ProveedorDto>> Crear(ProveedorRequest req, CancellationToken ct)
    {
        var creado = await _servicio.CrearAsync(req, ct);
        return CreatedAtAction(nameof(getSupplierById), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProveedorDto>> Editar(int id, ProveedorRequest req, CancellationToken ct)
        => Ok(await _servicio.EditarAsync(id, req, ct));

    [HttpPatch("{id:int}/desactivar")]
    public async Task<IActionResult> Desactivar(int id, CancellationToken ct)
    {
        await _servicio.DesactivarAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:int}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken ct)
    {
        await _servicio.ActivarAsync(id, ct);
        return NoContent();
    }
}