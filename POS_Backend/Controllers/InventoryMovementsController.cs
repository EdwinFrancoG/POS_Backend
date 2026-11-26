
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryMovementsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryMovementsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/InventoryMovements
        [HttpGet]
        public async Task<IActionResult> GetInventoryMovements()
        {
            var movements = await _context.Set<InventoryMovement>()
                .ToListAsync();

            return Ok(movements);
        }

        // GET: api/InventoryMovements/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryMovement(int id)
        {
            var movement = await _context.Set<InventoryMovement>()
                .FindAsync(id);

            if (movement == null)
                return NotFound();

            return Ok(movement);
        }

        // POST: api/InventoryMovements
        [HttpPost]
        public IActionResult PostInventoryMovement(InventoryMovement movement)
        {
            return StatusCode(501);
        }

        // PUT: api/InventoryMovements/5
        [HttpPut("{id}")]
        public IActionResult PutInventoryMovement(int id, InventoryMovement movement)
        {
            if (id != movement.Id)
                return BadRequest();

            return StatusCode(501);
        }

        // DELETE: api/InventoryMovements/5
        [HttpDelete("{id}")]
        public IActionResult DeleteInventoryMovement(int id)
        {
            return StatusCode(501);
        }
    }
}
