using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS_Backend.Datos;
using POS_Backend.Logica.Services;
using POS_Backend.Logica.Servicios;

namespace POS_Backend.Logica;

public static class DependencyInjection
{
    public static IServiceCollection AddLogica(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatos(configuration);
        services.AddScoped<ConexionService>();
        services.AddScoped<ProveedorService>();
        services.AddScoped<AsientoService>();
        return services;
    }
}