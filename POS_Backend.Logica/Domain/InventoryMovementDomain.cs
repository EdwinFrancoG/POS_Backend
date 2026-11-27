using POS_Backend.Logica.Constants;
using POS_Backend.Logica.Exceptions;

namespace POS_Backend.Logica.Domain;

// Reglas de negocio de los movimientos de inventario.
// Trabaja sobre el modelo InventoryMovement de la capa de datos. El acceso a la
// base de datos debe ir en un IInventoryMovementService, igual que en ProductDomain.
public class InventoryMovementDomain
{
    private static readonly string[] ValidTypes =
    {
        InventoryMovementType.Entry,
        InventoryMovementType.Exit,
        InventoryMovementType.Adjustment
    };

    public InventoryMovement CreateMovement(int productId, decimal quantity, string type, string? notes)
    {
        var errors = new List<string>();
        if (productId <= 0) errors.Add("Producto no válido.");
        if (quantity <= 0) errors.Add("La cantidad debe ser mayor que cero.");
        if (!ValidTypes.Contains(type)) errors.Add("Tipo de movimiento no válido.");
        if (errors.Count > 0) throw new BusinessRuleException(errors);

        return new InventoryMovement
        {
            ProductId = productId,
            Quantity = quantity,
            Type = type,
            Notes = notes,
            Date = DateTime.Now
        };
    }

    // Cuánto suma o resta el movimiento al stock del producto.
    public decimal GetStockEffect(InventoryMovement movement) => movement.Type switch
    {
        InventoryMovementType.Entry => movement.Quantity,
        InventoryMovementType.Exit => -movement.Quantity,
        _ => movement.Quantity
    };
}
