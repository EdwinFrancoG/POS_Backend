using Microsoft.AspNetCore.Mvc;
using POS_Backend.Logica.Domain;
using POS_Backend.Logica.Dtos;

namespace POS_Backend.Controllers;

[ApiController]
[Route("api/proveedores")]
public class SupplierController : ControllerBase
{
    private readonly SupplierDomain _supplierDomain;

    public SupplierController(SupplierDomain supplierDomain) => _supplierDomain = supplierDomain;

    [HttpGet]
    public async Task<ActionResult<List<SupplierDto>>> GetAll(
        [FromQuery] bool onlyActive = true, CancellationToken ct = default)
        => Ok(await _supplierDomain.GetAllAsync(onlyActive, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierDto>> GetById(int id, CancellationToken ct)
        => Ok(await _supplierDomain.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create(SupplierRequest request, CancellationToken ct)
    {
        var created = await _supplierDomain.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SupplierDto>> Update(int id, SupplierRequest request, CancellationToken ct)
        => Ok(await _supplierDomain.UpdateAsync(id, request, ct));

    [HttpPatch("{id:int}/desactivar")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
    {
        await _supplierDomain.DeactivateAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:int}/activar")]
    public async Task<IActionResult> Activate(int id, CancellationToken ct)
    {
        await _supplierDomain.ActivateAsync(id, ct);
        return NoContent();
    }
}