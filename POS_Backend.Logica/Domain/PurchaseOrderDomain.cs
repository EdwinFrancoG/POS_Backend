using System;
using System.Collections.Generic;
using System.Text;

namespace POS_Backend.Logica.Domain
{
    using Microsoft.EntityFrameworkCore;
    using POS_Backend.Datos;
    

    namespace POS_Backend.Business
    {
        public class PurchaseOrderBusiness
        {
            private readonly AppDbContext _context;

            public PurchaseOrderBusiness(AppDbContext db) => _context = db;
          

            // 1. CREAR ORDEN DE COMPRA
            public async Task<PurchaseOrder> CreateAsync(PurchaseOrder order)
            {
                if (order == null)
                    throw new Exception("La orden de compra es obligatoria.");

                if (string.IsNullOrWhiteSpace(order.Number))
                    throw new Exception("El número de orden es obligatorio.");

                if (order.SupplierId <= 0)
                    throw new Exception("Debe seleccionar un proveedor.");

                if (order.PurchaseOrderLines == null ||
                    !order.PurchaseOrderLines.Any())
                    throw new Exception("La orden debe contener productos.");

                bool exists = await _context.PurchaseOrders
                    .AnyAsync(x => x.Number == order.Number);

                if (exists)
                    throw new Exception("El número de orden ya existe.");

                var supplier = await _context.Suppliers
                    .FindAsync(order.SupplierId);

                if (supplier == null)
                    throw new Exception("El proveedor no existe.");

                decimal subtotal = 0;

                foreach (var line in order.PurchaseOrderLines)
                {
                    if (line.ProductId <= 0)
                        throw new Exception("Producto inválido.");

                    if (line.Quantity <= 0)
                        throw new Exception("La cantidad debe ser mayor que cero.");

                    if (line.UnitCost < 0)
                        throw new Exception("El costo no puede ser negativo.");

                    subtotal += line.Quantity * line.UnitCost;
                }

                order.Subtotal = subtotal;
                order.Tax = 0; // Configurar el impuesto según el negocio.
                order.Total = subtotal + order.Tax;

                order.Status = "Pendiente";
                order.AmountReceived = 0;
                order.AmoutPaid = 0;
                order.Date = DateTime.Now;

                _context.PurchaseOrders.Add(order);

                await _context.SaveChangesAsync();

                return order;
            }

            // 2. CONSULTAR TODAS LAS ORDENES
            public async Task<List<PurchaseOrder>> GetAllAsync()
            {
                return await _context.PurchaseOrders
                    .Include(x => x.Supplier)
                    .Include(x => x.PurchaseOrderLines)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();
            }

            // 3. CONSULTAR ORDEN POR ID
            public async Task<PurchaseOrder?> GetByIdAsync(int id)
            {
                return await _context.PurchaseOrders
                    .Include(x => x.Supplier)
                    .Include(x => x.PurchaseOrderLines)
                    .Include(x => x.PaymentAllocations)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }

            // 4. APROBAR ORDEN DE COMPRA
            public async Task<PurchaseOrder> ApproveAsync(
                int id, int userId)
            {
                var order = await _context.PurchaseOrders
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (order == null)
                    throw new Exception("Orden de compra no encontrada.");

                if (order.Status != "Pendiente")
                    throw new Exception(
                        "Solamente se pueden aprobar órdenes pendientes.");

                order.Status = "Aprobada";
                order.ApprovedById = userId;
                order.AprovedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return order;
            }

            // 5. RECIBIR PRODUCTOS
            public async Task<PurchaseOrder> ReceiveAsync(int id)
            {
                var order = await _context.PurchaseOrders
                    .Include(x => x.PurchaseOrderLines)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (order == null)
                    throw new Exception("Orden de compra no encontrada.");

                if (order.Status != "Aprobada")
                    throw new Exception(
                        "La orden debe estar aprobada antes de recibir productos.");

                using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    decimal totalReceived = 0;

                    foreach (var line in order.PurchaseOrderLines)
                    {
                        var product = await _context.Products
                            .FirstOrDefaultAsync(
                                x => x.Id == line.ProductId);

                        if (product == null)
                            throw new Exception(
                                $"No existe el producto ID {line.ProductId}.");

                        // Actualizar inventario
                        product.Stock += line.Quantity;

                        // Registrar movimiento de inventario
                        var movement = new InventoryMovement
                        {
                            ProductId = product.Id,
                            Type = "Entrada",
                            Date = DateTime.Now,
                            Quantity = line.Quantity,
                            UnitCost = line.UnitCost,
                            ResultingStock = product.Stock.ToString(),
                            PurchaseOrderId = order.Id,
                            Notes = "Entrada por orden de compra"
                        };

                        _context.InventoryMovements.Add(movement);

                        totalReceived += line.Quantity * line.UnitCost;
                    }

                    order.AmountReceived = order.PurchaseOrderLines
                        .Sum(x => x.Quantity);

                    order.Status = "Recibida";

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return order;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            // 6. REGISTRAR PAGO AL PROVEEDOR
            public async Task<PurchaseOrder> RegisterPaymentAsync(
                int id, decimal amount)
            {
                var order = await _context.PurchaseOrders
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (order == null)
                    throw new Exception("Orden de compra no encontrada.");

                if (amount <= 0)
                    throw new Exception("El pago debe ser mayor que cero.");

                decimal pending = order.Total - order.AmoutPaid;

                if (amount > pending)
                    throw new Exception(
                        "El pago supera el saldo pendiente.");

                order.AmoutPaid += amount;

                await _context.SaveChangesAsync();

                return order;
            }

            // 7. CONSULTAR SALDO PENDIENTE
            public async Task<decimal> GetPendingBalanceAsync(int id)
            {
                var order = await _context.PurchaseOrders
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (order == null)
                    throw new Exception("Orden no encontrada.");

                return order.Total - order.AmoutPaid;
            }

            // 8. CANCELAR ORDEN
            public async Task<bool> CancelAsync(int id)
            {
                var order = await _context.PurchaseOrders
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (order == null)
                    throw new Exception("Orden no encontrada.");

                if (order.Status == "Recibida")
                    throw new Exception(
                        "No se puede cancelar una orden recibida.");

                if (order.AmoutPaid > 0)
                    throw new Exception(
                        "No se puede cancelar una orden que tiene pagos registrados.");

                order.Status = "Cancelada";

                await _context.SaveChangesAsync();

                return true;
            }
        }
    }

}
