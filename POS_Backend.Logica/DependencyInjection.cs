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

        services.AddScoped<ProveedorService>();
        services.AddScoped<AsientoService>();
        services.AddScoped<CategoryDomain>();
        services.AddScoped<CategoryService>();
        services.AddScoped<ProductDomain>();
        services.AddScoped<ProductService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<CustomerDomain>();
        services.AddScoped<CustomerService>();

        return services;
    }
}