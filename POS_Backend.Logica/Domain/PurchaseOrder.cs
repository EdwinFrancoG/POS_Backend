using System;
using System.Collections.Generic;
using System.Text;

namespace POS_Backend.Logica.Domain

{

    public class PurchaseOrder
    {
        public int Id { get; private set; }

        public int SupplierId { get; private set; }

        public DateTime OrderDate { get; private set; }

        public string Status { get; private set; }

        public List<PurchaseOrderLine> Lines { get; private set; }

        public decimal Total =>
            Lines.Sum(line => line.Subtotal);

        public PurchaseOrder(int supplierId)
        {
            if (supplierId <= 0)
                throw new ArgumentException(
                    "Proveedor no válido");

            SupplierId = supplierId;
            OrderDate = DateTime.Now;
            Status = "Pendiente";
            Lines = new List<PurchaseOrderLine>();
        }

        public void AddLine(
            int productId,
            decimal quantity,
            decimal unitPrice)
        {
            if (Status != "Pendiente")
                throw new InvalidOperationException(
                    "La orden no está disponible para modificar");

            var line = new PurchaseOrderLine(
                productId, quantity, unitPrice);

            Lines.Add(line);
        }

        public void Cancel()
        {
            if (Status == "Recibida")
                throw new InvalidOperationException(
                    "No se puede cancelar una orden recibida");

            Status = "Cancelada";
        }

        public void MarkAsReceived()
        {
            if (Status != "Pendiente")
                throw new InvalidOperationException(
                    "La orden no está pendiente");

            if (!Lines.Any())
                throw new InvalidOperationException(
                    "La orden no tiene productos");

            Status = "Recibida";
        }
    }
}

