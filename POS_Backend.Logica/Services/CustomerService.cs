using System;
using System.Collections.Generic;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;
using POS_Backend.Logica.Domain;

namespace POS_Backend.Logica.Services
{
    public class CustomerService
    {
        private readonly CustomerDomain _customerDomain;
        private readonly AppDbContext _db;
        public CustomerService(CustomerDomain customerDomain, AppDbContext db)
        {
            _customerDomain = customerDomain;
            _db = db;
        }
        // Consultas
        public async Task<IEnumerable<Customers>> ObtenerTodos(bool soloActivos = true)
        {
            var response = await _customerDomain.ObtenerTodos(soloActivos);
            return response;
        }

        public async Task<Customers?> ObtenerPorId(int id)
        {
            var response = await _customerDomain.ObtenerPorId(id);
            return response;
        }

        public async Task<IEnumerable<Customers>> Buscar(string texto)
        {
            var response = await _customerDomain.Buscar(texto);
            return response;
        }
        // Operaciones
        public async Task<string> Crear(Customers cliente)
        {
            var response = await _customerDomain.VerificarCreacion(cliente);

            if (response == "OK")
            {
                try
                {
                    cliente.IsActive = true;
                    _db.Customers.Add(cliente);
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
        public async Task<string> Actualizar(Customers cliente)
        {
            var response = await _customerDomain.VerificarActualizacion(cliente);

            if (response == "OK")
            {
                try
                {
                    var existente = await _db.Customers.FindAsync(cliente.Id);

                    existente!.Code = cliente.Code;
                    existente.Name = cliente.Name;
                    existente.Rtn = cliente.Rtn;
                    existente.Phone = cliente.Phone;
                    existente.Email = cliente.Email;
                    existente.Address = cliente.Address;
                    existente.CreditLimit = cliente.CreditLimit;
                    existente.CreditDays = cliente.CreditDays;

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
            var response = await _customerDomain.VerificarCambioEstado(id);

            if (response == "OK")
            {
                try
                {
                    var cliente = await _db.Customers.FindAsync(id);
                    cliente!.IsActive = activo;

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
