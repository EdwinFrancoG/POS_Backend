using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceLinesController : ControllerBase
{
    private readonly AppDbContext _context;

    public InvoiceLinesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/InvoiceLines
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceLines>>> GetInvoiceLines()
    {
        return await _context.InvoiceLines.ToListAsync();
    }

    // GET: api/InvoiceLines/5
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceLines>> GetInvoiceLine(int id)
    {
        var invoiceLine = await _context.InvoiceLines.FindAsync(id);

        if (invoiceLine == null)
        {
            return NotFound();
        }

        return invoiceLine;
    }

    // POST: api/InvoiceLines
    [HttpPost]
    public async Task<ActionResult<InvoiceLines>> PostInvoiceLine(InvoiceLines invoiceLine)
    {
        _context.InvoiceLines.Add(invoiceLine);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetInvoiceLine),
            new { id = invoiceLine.Id },
            invoiceLine);
    }

    // PUT: api/InvoiceLines/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutInvoiceLine(int id, InvoiceLines invoiceLine)
    {
        if (id != invoiceLine.Id)
        {
            return BadRequest();
        }

        _context.Entry(invoiceLine).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/InvoiceLines/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoiceLine(int id)
    {
        var invoiceLine = await _context.InvoiceLines.FindAsync(id);

        if (invoiceLine == null)
        {
            return NotFound();
        }

        _context.InvoiceLines.Remove(invoiceLine);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}