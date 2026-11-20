using System;
using System.Collections.Generic;
using System.Text;

namespace POS_Backend.Logica.Domain
{
    using global::POS_Backend.Datos;
    using Microsoft.EntityFrameworkCore;
    using POS_Backend.Datos

    public class PurchaseOrderLineBusiness
        {
            private readonly AppDbContext _context;

            public PurchaseOrderLineBusiness(AppDbContext db) => _context = db;
           

            // 1. AGREGAR PRODUCTO A LA ORDEN
            public async Task<PurchaseOrderLine> CreateAsync(
                PurchaseOrderLine line)
            {
                if (line == null)
                    throw new Exception("La línea es obligatoria.");

                if (line.PurchaseOrderId <= 0)
                    throw new Exception("Orden de compra inválida.");

                if (line.ProductId <= 0)
                    throw new Exception("Producto inválido.");

                if (line.Quantity <= 0)
                    throw new Exception("La cantidad debe ser mayor que cero.");

                if (line.UnitCost < 0)
                    throw new Exception("El costo no puede ser negativo.");

                if (line.TaxRate < 0 || line.TaxRate > 100)
                    throw new Exception("El impuesto debe estar entre 0 y 100.");

                var order = await _context.PurchaseOrders
                    .Include(x => x.PurchaseOrderLines)
                    .FirstOrDefaultAsync(x =>
                        x.Id == line.PurchaseOrderId);

                if (order == null)
                    throw new Exception("Orden de compra no encontrada.");

                if (order.Status != "Pendiente")
                    throw new Exception(
                        "Solo se pueden modificar órdenes pendientes.");

                var product = await _context.Products
                    .FindAsync(line.ProductId);

                if (product == null)
                    throw new Exception("El producto no existe.");

                line.ReceivedQuantity = 0;

                CalculateLine(line);

                _context.PurchaseOrderLines.Add(line);

                await _context.SaveChangesAsync();

                await RecalculateOrderAsync(order.Id);

                return line;
            }

            // 2. CALCULAR SUBTOTAL DE LA LÍNEA
            private void CalculateLine(PurchaseOrderLine line)
            {
                decimal subtotal = line.Quantity * line.UnitCost;

                decimal tax = subtotal * (line.TaxRate / 100);

                line.Subtotal = Math.Round(
                    subtotal + tax, 2,
                    MidpointRounding.AwayFromZero);
            }

            // 3. CONSULTAR TODAS LAS LÍNEAS
            public async Task<List<PurchaseOrderLine>> GetAllAsync()
            {
                return await _context.PurchaseOrderLines
                    .Include(x => x.PurchaseOrder)
                    .Include(x => x.PurchaseOrder.Supplier)
                    .ToListAsync();
            }

            // 4. CONSULTAR LÍNEAS POR ORDEN
            public async Task<List<PurchaseOrderLine>> GetByOrderIdAsync(
                int orderId)
            {
                return await _context.PurchaseOrderLines
                    .Where(x => x.PurchaseOrderId == orderId)
                    .ToListAsync();
            }

            // 5. CONSULTAR LÍNEA POR ID
            public async Task<PurchaseOrderLine?> GetByIdAsync(int id)
            {
                return await _context.PurchaseOrderLines
                    .FirstOrDefaultAsync(x => x.Id == id);
            }

            // 6. ACTUALIZAR PRODUCTO DE LA ORDEN
            public async Task<PurchaseOrderLine> UpdateAsync(
                int id, PurchaseOrderLine updated)
            {
                var line = await _context.PurchaseOrderLines
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (line == null)
                    throw new Exception("Línea no encontrada.");

                if (line.ReceivedQuantity > 0)
                    throw new Exception(
                        "No se puede modificar una línea que ya tiene productos recibidos.");

                if (updated.Quantity <= 0)
                    throw new Exception("La cantidad debe ser mayor que cero.");

                if (updated.UnitCost < 0)
                    throw new Exception("El costo no puede ser negativo.");

                if (updated.TaxRate < 0 || updated.TaxRate > 100)
                    throw new Exception("Impuesto inválido.");

                var order = await _context.PurchaseOrders
                    .FirstOrDefaultAsync(x =>
                        x.Id == line.PurchaseOrderId);

                if (order == null || order.Status != "Pendiente")
                    throw new Exception(
                        "La orden no permite modificaciones.");

                line.Quantity = updated.Quantity;
                line.UnitCost = updated.UnitCost;
                line.TaxRate = updated.TaxRate;

                CalculateLine(line);

                await _context.SaveChangesAsync();

                await RecalculateOrderAsync(line.PurchaseOrderId);

                return line;
            }

            // 7. REGISTRAR RECEPCIÓN PARCIAL O TOTAL
            public async Task<PurchaseOrderLine> ReceiveAsync(
                int id, decimal receivedQuantity)
            {
                var line = await _context.PurchaseOrderLines
                    .Include(x => x.PurchaseOrder)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (line == null)
                    throw new Exception("Línea no encontrada.");

                if (line.PurchaseOrder.Status != "Aprobada" &&
                    line.PurchaseOrder.Status != "Parcialmente recibida")
                    throw new Exception(
                        "La orden debe estar aprobada para recibir productos.");

                if (receivedQuantity <= 0)
                    throw new Exception(
                        "La cantidad recibida debe ser mayor que cero.");

                decimal pending = line.Quantity - line.ReceivedQuantity;

                if (receivedQuantity > pending)
                    throw new Exception(
                        "La cantidad recibida supera la cantidad pendiente.");

                using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    var product = await _context.Products
                        .FindAsync(line.ProductId);

                    if (product == null)
                        throw new Exception("Producto no encontrado.");

                    line.ReceivedQuantity += receivedQuantity;

                    product.Stock += receivedQuantity;

                    var movement = new InventoryMovement
                    {
                        ProductId = product.Id,
                        Type = "Entrada",
                        Date = DateTime.Now,
                        Quantity = receivedQuantity,
                        UnitCost = line.UnitCost,
                        ResultingStock = product.Stock.ToString(),
                        PurchaseOrderId = line.PurchaseOrderId,
                        Notes = "Recepción de orden de compra"
                    };

                    _context.InventoryMovements.Add(movement);

                    await _context.SaveChangesAsync();

                    await RecalculateOrderAsync(line.PurchaseOrderId);

                    await transaction.CommitAsync();

                    return line;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            // 8. RECALCULAR TOTALES DE LA ORDEN
            private async Task RecalculateOrderAsync(int orderId)
            {
                var order = await _context.PurchaseOrders
                    .Include(x => x.PurchaseOrderLines)
                    .FirstOrDefaultAsync(x => x.Id == orderId);

                if (order == null)
                    throw new Exception("Orden no encontrada.");

                order.Subtotal = order.PurchaseOrderLines.Sum(
                    x => x.Quantity * x.UnitCost);

                order.Tax = order.PurchaseOrderLines.Sum(
                    x => x.Subtotal - (x.Quantity * x.UnitCost));

                order.Total = order.Subtotal + order.Tax;

                decimal totalReceived = order.PurchaseOrderLines.Sum(
                    x => x.ReceivedQuantity * x.UnitCost);

                order.AmountReceived = totalReceived;

                bool allReceived = order.PurchaseOrderLines.Any() &&
                    order.PurchaseOrderLines.All(
                        x => x.ReceivedQuantity == x.Quantity);

                bool someReceived = order.PurchaseOrderLines.Any(
                    x => x.ReceivedQuantity > 0);

                if (allReceived)
                    order.Status = "Recibida";
                else if (someReceived)
                    order.Status = "Parcialmente recibida";

                await _context.SaveChangesAsync();
            }

            // 9. ELIMINAR LÍNEA
            public async Task<bool> DeleteAsync(int id)
            {
                var line = await _context.PurchaseOrderLines
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (line == null)
                    throw new Exception("Línea no encontrada.");

                if (line.ReceivedQuantity > 0)
                    throw new Exception(
                        "No se puede eliminar una línea que ya tiene productos recibidos.");

                var order = await _context.PurchaseOrders
                    .FirstOrDefaultAsync(x =>
                        x.Id == line.PurchaseOrderId);

                if (order == null || order.Status != "Pendiente")
                    throw new Exception(
                        "No se puede modificar esta orden.");

                _context.PurchaseOrderLines.Remove(line);

                await _context.SaveChangesAsync();

                await RecalculateOrderAsync(line.PurchaseOrderId);

                return true;
            }
        }
    }


