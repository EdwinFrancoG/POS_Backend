using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicesController : ControllerBase
{
    private readonly AppDbContext _context;

    public InvoicesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Invoices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invoices>>> GetInvoices()
    {
        return await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.InvoiceLines)
            .Include(i => i.Payments)
            .ToListAsync();
    }

    // GET: api/Invoices/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Invoices>> GetInvoice(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.InvoiceLines)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
        {
            return NotFound();
        }

        return invoice;
    }

    // POST: api/Invoices
    [HttpPost]
    public async Task<ActionResult<Invoices>> PostInvoice(Invoices invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetInvoice),
            new { id = invoice.Id },
            invoice);
    }

    // PUT: api/Invoices/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutInvoice(int id, Invoices invoice)
    {
        if (id != invoice.Id)
        {
            return BadRequest();
        }

        _context.Entry(invoice).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Invoices/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);

        if (invoice == null)
        {
            return NotFound();
        }

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}