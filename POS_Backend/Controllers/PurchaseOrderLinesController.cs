
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace POS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderLinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseOrderLinesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<PurchaseOrderLinesController>
        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrderLines()
        {
            var lines = await _context.Set<PurchaseOrderLine>()
                .ToListAsync();

            return Ok(lines);
        }

        // GET api/<PurchaseOrderLinesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderLine(int id)
        {
            var line = await _context.Set<PurchaseOrderLine>()
                .FindAsync(id);

            if (line == null)
                return NotFound();

            return Ok(line);
        }
        // POST api/<PurchaseOrderLinesController>
        [HttpPost]
        public IActionResult PostPurchaseOrderLine(
             [FromBody] PurchaseOrderLine line)
        {  
            
            // Pendiente de integrar las reglas de compra.
            return StatusCode(501);
        }


        // PUT api/<PurchaseOrderLinesController>/5
        [HttpPut("{id}")]
        public IActionResult PutPurchaseOrderLine(
            int id, [FromBody] PurchaseOrderLine line)
        {
            if (id != line.Id)
                return BadRequest();

            // Pendiente de actualizar los totales de la orden.
            return StatusCode(501);
        }

        // DELETE api/<PurchaseOrderLinesController>/5
        [HttpDelete("{id}")]
        public IActionResult DeletePurchaseOrderLine(int id)
        {
            // Pendiente de integrar las reglas de eliminación.
            return StatusCode(501);
        }
    }
}
