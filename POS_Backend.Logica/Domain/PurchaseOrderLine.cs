using System;
using System.Collections.Generic;
using System.Text;

namespace POS_Backend.Logica.Domain
{


    
        public class PurchaseOrderLine
        {
            public int Id { get; private set; }

            public int ProductId { get; private set; }

            public decimal Quantity { get; private set; }

            public decimal UnitPrice { get; private set; }

            public decimal Subtotal =>
                Quantity * UnitPrice;

            public PurchaseOrderLine(
                int productId,
                decimal quantity,
                decimal unitPrice)
            {
                if (productId <= 0)
                    throw new ArgumentException(
                        "Producto no válido");

                if (quantity <= 0)
                    throw new ArgumentException(
                        "La cantidad debe ser mayor que cero");

                if (unitPrice < 0)
                    throw new ArgumentException(
                        "El precio no puede ser negativo");

                ProductId = productId;
                Quantity = quantity;
                UnitPrice = unitPrice;
            }
        }
    }
