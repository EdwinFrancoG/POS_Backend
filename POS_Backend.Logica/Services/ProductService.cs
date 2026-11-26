using System;
using System.Collections.Generic;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;
using POS_Backend.Logica.Domain;

namespace POS_Backend.Logica.Services
{
    public class ProductService
    {
        private readonly ProductDomain _productDomain;
        private readonly AppDbContext _db;

        public ProductService(ProductDomain productDomain, AppDbContext db)
        {
            _productDomain = productDomain;
            _db = db;
        }

        // Consultas
        public async Task<IEnumerable<Product>> ObtenerTodos(bool soloActivos = true)
        {
            var response = await _productDomain.ObtenerTodos(soloActivos);
            return response;
        }

        public async Task<Product?> ObtenerPorId(int id)
        {
            var response = await _productDomain.ObtenerPorId(id);
            return response;
        }

        public async Task<IEnumerable<Product>> Buscar(string texto)
        {
            var response = await _productDomain.Buscar(texto);
            return response;
        }

        public async Task<IEnumerable<Product>> ObtenerStockBajo()
        {
            var response = await _productDomain.ObtenerStockBajo();
            return response;
        }

        // Operaciones
        public async Task<string> Crear(Product producto)
        {
            var response = await _productDomain.VerificarCreacion(producto);

            if (response == "OK")
            {
                try
                {
                    producto.IsActive = true;
                    _db.Products.Add(producto);
                    await _db.SaveChangesAsync();
                    return "OK";
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return response;
        }

        public async Task<string> Actualizar(Product producto)
        {
            var response = await _productDomain.VerificarActualizacion(producto);

            if (response == "OK")
            {
                try
                {
                    var existente = await _db.Products.FindAsync(producto.Id);

                    existente!.Code = producto.Code;
                    existente.Barcode = producto.Barcode;
                    existente.Name = producto.Name;
                    existente.Description = producto.Description;
                    existente.UnitOfMeasure = producto.UnitOfMeasure;
                    existente.CategoryId = producto.CategoryId;
                    existente.SupplierId = producto.SupplierId;
                    existente.AverageCost = producto.AverageCost;
                    existente.SalePrice = producto.SalePrice;
                    existente.TaxRate = producto.TaxRate;
                    existente.MinimumStock = producto.MinimumStock;

                    await _db.SaveChangesAsync();
                    return "OK";
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return response;
        }

        public async Task<string> CambiarEstado(int id, bool activo)
        {
            var response = await _productDomain.VerificarCambioEstado(id);

            if (response == "OK")
            {
                try
                {
                    var producto = await _db.Products.FindAsync(id);
                    producto!.IsActive = activo;

                    await _db.SaveChangesAsync();
                    return "OK";
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return response;
        }
    }
}
