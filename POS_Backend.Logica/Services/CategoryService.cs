using System;
using System.Collections.Generic;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;
using POS_Backend.Logica.Domain;

namespace POS_Backend.Logica.Services
{
    public class CategoryService
    {
        private readonly CategoryDomain _categoryDomain;
        private readonly AppDbContext _db;

        public CategoryService(CategoryDomain categoryDomain, AppDbContext db)
        {
            _categoryDomain = categoryDomain;
            _db = db;
        }

        // Consultas
        public async Task<IEnumerable<Category>> ObtenerTodas(bool soloActivas = true)
        {
            var response = await _categoryDomain.ObtenerTodas(soloActivas);
            return response;
        }

        public async Task<Category?> ObtenerPorId(int id)
        {
            var response = await _categoryDomain.ObtenerPorId(id);
            return response;
        }

        // Operaciones
        public async Task<string> Crear(Category categoria)
        {
            var response = await _categoryDomain.VerificarCreacion(categoria);

            if (response == "OK")
            {
                try
                {
                    categoria.IsActive = true;
                    _db.Categories.Add(categoria);
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

        public async Task<string> Actualizar(Category categoria)
        {
            var response = await _categoryDomain.VerificarActualizacion(categoria);

            if (response == "OK")
            {
                try
                {
                    var existente = await _db.Categories.FindAsync(categoria.Id);

                    existente!.Name = categoria.Name;
                    existente.Description = categoria.Description;

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

        public async Task<string> CambiarEstado(int id, bool activa)
        {
            var response = await _categoryDomain.VerificarCambioEstado(id, activa);

            if (response == "OK")
            {
                try
                {
                    var categoria = await _db.Categories.FindAsync(id);
                    categoria!.IsActive = activa;

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