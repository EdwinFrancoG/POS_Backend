using POS_Backend.Datos.Models;
using POS_Backend.Logica.Constants;
using POS_Backend.Logica.Exceptions;

namespace POS_Backend.Logica.Domain;

// Reglas de negocio de las órdenes de compra.
// Trabaja sobre los modelos de la capa de datos. El acceso a la base de datos
// debe ir en un IPurchaseOrderService, igual que en ProductDomain.
public class PurchaseOrderDomain
{
    public PurchaseOrder CreateOrder(int supplierId)
    {
        if (supplierId <= 0)
            throw new BusinessRuleException("Proveedor no válido.");

        return new PurchaseOrder
        {
            SupplierId = supplierId,
            Date = DateTime.Now,
            Status = PurchaseOrderStatus.Pending,
            PurchaseOrderLines = new List<PurchaseOrderLine>()
        };
    }

    public PurchaseOrderLine AddLine(PurchaseOrder order, int productId, decimal quantity, decimal unitCost)
    {
        if (order.Status != PurchaseOrderStatus.Pending)
            throw new BusinessRuleException("La orden no está disponible para modificar.");

        var errors = new List<string>();
        if (productId <= 0) errors.Add("Producto no válido.");
        if (quantity <= 0) errors.Add("La cantidad debe ser mayor que cero.");
        if (unitCost < 0) errors.Add("El precio no puede ser negativo.");
        if (errors.Count > 0) throw new BusinessRuleException(errors);

        var line = new PurchaseOrderLine
        {
            ProductId = productId,
            Quantity = quantity,
            UnitCost = unitCost,
            Subtotal = quantity * unitCost
        };

        order.PurchaseOrderLines ??= new List<PurchaseOrderLine>();
        order.PurchaseOrderLines.Add(line);
        RecalculateTotals(order);

        return line;
    }

    public void Cancel(PurchaseOrder order)
    {
        if (order.Status == PurchaseOrderStatus.Received)
            throw new BusinessRuleException("No se puede cancelar una orden recibida.");

        order.Status = PurchaseOrderStatus.Cancelled;
    }

    public void MarkAsReceived(PurchaseOrder order)
    {
        if (order.Status != PurchaseOrderStatus.Pending)
            throw new BusinessRuleException("La orden no está pendiente.");

        if (order.PurchaseOrderLines is null || order.PurchaseOrderLines.Count == 0)
            throw new BusinessRuleException("La orden no tiene productos.");

        order.Status = PurchaseOrderStatus.Received;
    }

    // El total es la suma de los subtotales de las líneas.
    // Los impuestos se agregarán cuando se defina esa regla.
    private static void RecalculateTotals(PurchaseOrder order)
    {
        order.Subtotal = order.PurchaseOrderLines.Sum(l => l.Subtotal);
        order.Total = order.Subtotal;
    }
}
