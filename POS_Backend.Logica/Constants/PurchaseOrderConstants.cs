namespace POS_Backend.Logica.Constants;

public static class PurchaseOrderStatus
{
    public const string Pending = "Pendiente";
    public const string Received = "Recibida";
    public const string Cancelled = "Cancelada";
}

public static class InventoryMovementType
{
    public const string Entry = "Entrada";
    public const string Exit = "Salida";
    public const string Adjustment = "Ajuste";
}
