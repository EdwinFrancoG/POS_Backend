
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalEntryLinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JournalEntryLinesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/JournalEntryLines
        [HttpGet]
        public async Task<IActionResult> GetJournalEntryLines()
        {
            var lines = await _context.Set<JournalEntryLine>()
                .ToListAsync();

            return Ok(lines);
        }

        // GET: api/JournalEntryLines/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJournalEntryLine(int id)
        {
            var line = await _context.Set<JournalEntryLine>()
                .FindAsync(id);

            if (line == null)
                return NotFound();

            return Ok(line);
        }

        // POST: api/JournalEntryLines
        [HttpPost]
        public IActionResult PostJournalEntryLine(
            [FromBody] JournalEntryLine line)
        {
            if (line.Debit < 0 || line.Credit < 0)
                return BadRequest("Los valores no pueden ser negativos.");

            if (line.Debit > 0 && line.Credit > 0)
                return BadRequest("No puede registrar debito y credito en la misma linea.");

            if (line.Debit == 0 && line.Credit == 0)
                return BadRequest("Debe registrar un debito o un credito.");

            // Pendiente de integrar la logica contable.
            return StatusCode(501);
        }

        // PUT: api/JournalEntryLines/5
        [HttpPut("{id}")]
        public IActionResult PutJournalEntryLine(
            int id, [FromBody] JournalEntryLine line)
        {
            if (id != line.Id)
                return BadRequest();

            // Pendiente de validar el estado del asiento contable.
            return StatusCode(501);
        }

        // DELETE: api/JournalEntryLines/5
        [HttpDelete("{id}")]
        public IActionResult DeleteJournalEntryLine(int id)
        {
            // Pendiente de integrar las reglas contables.
            return StatusCode(501);
        }
    }
}
