using Microsoft.AspNetCore.Mvc;
using POS_Backend.Logica.Domain;
using POS_Backend.Logica.Dtos;

namespace POS_Backend.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductController : ControllerBase
{
    private readonly ProductDomain _productDomain;

    public ProductController(ProductDomain productDomain) => _productDomain = productDomain;

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll(
        [FromQuery] bool onlyActive = true, CancellationToken ct = default)
        => Ok(await _productDomain.GetAllAsync(onlyActive, ct));

    [HttpGet("buscar")]
    public async Task<ActionResult<List<ProductDto>>> Search(
        [FromQuery] string? text, CancellationToken ct)
        => Ok(await _productDomain.SearchAsync(text, ct));

    [HttpGet("stock-bajo")]
    public async Task<ActionResult<List<ProductDto>>> GetLowStock(CancellationToken ct)
        => Ok(await _productDomain.GetLowStockAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken ct)
        => Ok(await _productDomain.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(ProductRequest request, CancellationToken ct)
    {
        var created = await _productDomain.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, ProductRequest request, CancellationToken ct)
        => Ok(await _productDomain.UpdateAsync(id, request, ct));

    [HttpPatch("{id:int}/desactivar")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
    {
        await _productDomain.DeactivateAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:int}/activar")]
    public async Task<IActionResult> Activate(int id, CancellationToken ct)
    {
        await _productDomain.ActivateAsync(id, ct);
        return NoContent();
    }
}