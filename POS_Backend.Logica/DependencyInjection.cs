using POS_Backend.Logica.Servicios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS_Backend.Datos;

namespace POS_Backend.Logica;

public static class DependencyInjection
{
    public static IServiceCollection AddLogica(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatos(configuration);
        services.AddScoped<ConexionService>();
        return services;
    }
}