
namespace POS_Backend.Logica.Domain
{
    using POS_Backend.Datos;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class InventoryMovementBusiness
    {
        private readonly AppDbContext _context;

        public InventoryMovementBusiness(AppDbContext db) => _context = db;
       

        // Registrar un movimiento de inventario
        public async Task<InventoryMovement> RegisterMovement(
            InventoryMovement movement)
        {
            // Validar cantidad
            if (movement.Quantity <= 0)
                throw new Exception("La cantidad debe ser mayor que cero.");

            // Validar costo
            if (movement.UnitCost < 0)
                throw new Exception("El costo no puede ser negativo.");

            // Buscar producto
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == movement.ProductId);

            if (product == null)
                throw new Exception("El producto no existe.");

            // Obtener stock actual
            decimal currentStock = product.Stock;

            // Validar tipo de movimiento
            if (movement.Type == "Entrada")
            {
                currentStock += movement.Quantity;
            }
            else if (movement.Type == "Salida")
            {
                if (currentStock < movement.Quantity)
                    throw new Exception("No hay suficiente stock disponible.");

                currentStock -= movement.Quantity;
            }
            else
            {
                throw new Exception("Tipo de movimiento inválido.");
            }

            // Actualizar datos del movimiento
            movement.Date = DateTime.Now;
            movement.ResultingStock = currentStock.ToString();

            // Actualizar inventario
            product.Stock = currentStock;

            // Guardar movimiento
            _context.InventoryMovements.Add(movement);

            await _context.SaveChangesAsync();

            return movement;
        }

        // Consultar todos los movimientos
        public async Task<List<InventoryMovement>> GetMovements()
        {
            return await _context.InventoryMovements
                .Include(m => m.Product)
                .OrderByDescending(m => m.Date)
                .ToListAsync();
        }

        // Consultar movimientos por producto
        public async Task<List<InventoryMovement>> GetByProduct(int productId)
        {
            return await _context.InventoryMovements
                .Include(m => m.Product)
                .Where(m => m.ProductId == productId)
                .OrderByDescending(m => m.Date)
                .ToListAsync();
        }

        // Consultar movimientos por tipo
        public async Task<List<InventoryMovement>> GetByType(string type)
        {
            return await _context.InventoryMovements
                .Include(m => m.Product)
                .Where(m => m.Type == type)
                .ToListAsync();
        }

        // Consultar movimiento por ID
        public async Task<InventoryMovement> GetById(int id)
        {
            var movement = await _context.InventoryMovements
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movement == null)
                throw new Exception("Movimiento no encontrado.");

            return movement;
        }
    }

}
