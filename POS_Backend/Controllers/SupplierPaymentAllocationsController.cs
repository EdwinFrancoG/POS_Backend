
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierPaymentAllocationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SupplierPaymentAllocationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SupplierPaymentAllocations
        [HttpGet]
        public async Task<IActionResult> GetSupplierPaymentAllocations()
        {
            var allocations = await _context.Set<SupplierPaymentAllocation>()
                .ToListAsync();

            return Ok(allocations);
        }

        // GET: api/SupplierPaymentAllocations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierPaymentAllocation(int id)
        {
            var allocation = await _context.Set<SupplierPaymentAllocation>()
                .FindAsync(id);

            if (allocation == null)
                return NotFound();

            return Ok(allocation);
        }

        // POST: api/SupplierPaymentAllocations
        [HttpPost]
        public IActionResult PostSupplierPaymentAllocation(
            [FromBody] SupplierPaymentAllocation allocation)
        {
            // Pendiente de integrar la logica de asignaciones.
            return StatusCode(501);
        }

        // PUT: api/SupplierPaymentAllocations/5
        [HttpPut("{id}")]
        public IActionResult PutSupplierPaymentAllocation(
            int id, [FromBody] SupplierPaymentAllocation allocation)
        {
            if (id != allocation.Id)
                return BadRequest();

            // Pendiente de integrar las reglas de pagos.
            return StatusCode(501);
        }

        // DELETE: api/SupplierPaymentAllocations/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSupplierPaymentAllocation(int id)
        {
            // Pendiente de integrar la anulacion de asignaciones.
            return StatusCode(501);
        }
    }
}
