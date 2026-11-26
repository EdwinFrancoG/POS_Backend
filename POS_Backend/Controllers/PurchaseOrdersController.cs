
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseOrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PurchaseOrders
        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrders()
        {
            var orders = await _context.Set<PurchaseOrder>()
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/PurchaseOrders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrder(int id)
        {
            var order = await _context.Set<PurchaseOrder>()
                .FindAsync(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // POST: api/PurchaseOrders
        [HttpPost]
        public async Task<IActionResult> PostPurchaseOrder(
            [FromBody] PurchaseOrder order)
        {
            _context.Set<PurchaseOrder>().Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPurchaseOrder),
                new { id = order.Id },
                order);
        }

        // PUT: api/PurchaseOrders/5
        [HttpPut("{id}")]
        public IActionResult PutPurchaseOrder(
            int id, [FromBody] PurchaseOrder order)
        {
            if (id != order.Id)
                return BadRequest();

            // Pendiente de integrar las reglas de compra.
            return StatusCode(501);
        }

        // DELETE: api/PurchaseOrders/5
        [HttpDelete("{id}")]
        public IActionResult DeletePurchaseOrder(int id)
        {
            // Pendiente de integrar la anulación de órdenes.
            return StatusCode(501);
        }
    }
}
