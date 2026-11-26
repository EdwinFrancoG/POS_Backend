
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierPaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SupplierPaymentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SupplierPayments
        [HttpGet]
        public async Task<IActionResult> GetSupplierPayments()
        {
            var payments = await _context.Set<SupplierPayment>()
                .ToListAsync();

            return Ok(payments);
        }

        // GET: api/SupplierPayments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierPayment(int id)
        {
            var payment = await _context.Set<SupplierPayment>()
                .FindAsync(id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        // POST: api/SupplierPayments
        [HttpPost]
        public IActionResult PostSupplierPayment(
            [FromBody] SupplierPayment payment)
        {
            // Pendiente de integrar la lógica de pagos.
            return StatusCode(501);
        }

        // PUT: api/SupplierPayments/5
        [HttpPut("{id}")]
        public IActionResult PutSupplierPayment(
            int id, [FromBody] SupplierPayment payment)
        {
            if (id != payment.Id)
                return BadRequest();

            // Pendiente de integrar las reglas contables.
            return StatusCode(501);
        }

        // DELETE: api/SupplierPayments/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSupplierPayment(int id)
        {
            // Pendiente de integrar la anulación del pago.
            return StatusCode(501);
        }
    }
}
