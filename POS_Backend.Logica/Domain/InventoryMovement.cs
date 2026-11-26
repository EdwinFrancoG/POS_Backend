using System;
using System.Collections.Generic;
using System.Text;

namespace POS_Backend.Logica.Domain
{

   
        public class InventoryMovement
        {
            public int Id { get; private set; }

            public int ProductId { get; private set; }

            public decimal Quantity { get; private set; }

            public string MovementType { get; private set; }

            public DateTime MovementDate { get; private set; }

            public string Description { get; private set; }

            public InventoryMovement(
                int productId,
                decimal quantity,
                string movementType,
                string description)
            {
                if (productId <= 0)
                    throw new ArgumentException(
                        "Producto no válido");

                if (quantity <= 0)
                    throw new ArgumentException(
                        "La cantidad debe ser mayor que cero");

                if (movementType != "Entrada" &&
                    movementType != "Salida" &&
                    movementType != "Ajuste")
                {
                    throw new ArgumentException(
                        "Tipo de movimiento no válido");
                }

                ProductId = productId;
                Quantity = quantity;
                MovementType = movementType;
                Description = description;
                MovementDate = DateTime.Now;
            }

            public decimal GetStockEffect()
            {
                if (MovementType == "Entrada")
                    return Quantity;

                if (MovementType == "Salida")
                    return -Quantity;

                return Quantity;
            }
        }
    }
