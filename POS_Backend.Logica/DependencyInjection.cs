using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS_Backend.Datos;
using POS_Backend.Logica.Domain;
using POS_Backend.Logica.Services;

namespace POS_Backend.Logica;

public static class DependencyInjection
{
    public static IServiceCollection AddLogica(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatos(configuration);
        services.AddScoped<IDatabaseConnectionService, DatabaseConnectionService>();
        services.AddScoped<DatabaseConnectionDomain>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<SupplierDomain>();
        services.AddScoped<IJournalEntryService, JournalEntryService>();
        services.AddScoped<JournalEntryDomain>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ProductDomain>();
        services.AddScoped<CategoryDomain>();
        services.AddScoped<CategoryService>();
        return services;
    }
}