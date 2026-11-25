using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceNumberRangesController : ControllerBase
{
    private readonly AppDbContext _context;

    public InvoiceNumberRangesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/InvoiceNumberRanges
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceNumberRanges>>> GetInvoiceNumberRanges()
    {
        return await _context.InvoiceNumberRanges.ToListAsync();
    }

    // GET: api/InvoiceNumberRanges/5
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceNumberRanges>> GetInvoiceNumberRange(int id)
    {
        var range = await _context.InvoiceNumberRanges.FindAsync(id);

        if (range == null)
            return NotFound();

        return range;
    }

    // POST: api/InvoiceNumberRanges
    [HttpPost]
    public async Task<ActionResult<InvoiceNumberRanges>> PostInvoiceNumberRange(InvoiceNumberRanges range)
    {
        _context.InvoiceNumberRanges.Add(range);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInvoiceNumberRange),
            new { id = range.Id }, range);
    }

    // PUT: api/InvoiceNumberRanges/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutInvoiceNumberRange(int id, InvoiceNumberRanges range)
    {
        if (id != range.Id)
            return BadRequest();

        _context.Entry(range).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/InvoiceNumberRanges/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoiceNumberRange(int id)
    {
        var range = await _context.InvoiceNumberRanges.FindAsync(id);

        if (range == null)
            return NotFound();

        _context.InvoiceNumberRanges.Remove(range);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}