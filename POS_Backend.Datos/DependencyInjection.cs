using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace POS_Backend.Datos;

public static class DependencyInjection
{
    public static IServiceCollection AddDatos(this IServiceCollection services, IConfiguration configuration)
    {
        var proveedor = configuration["Database:Proveedor"] ?? "SqlServer";
        var modo = configuration["Database:Modo"] ?? "Local";

        var connectionString = configuration.GetConnectionString(modo)
            ?? throw new InvalidOperationException($"No existe ConnectionStrings:{modo} en appsettings.");

        services.AddDbContext<AppDbContext>(options =>
        {
            switch (proveedor)
            {
                case "SqlServer":
                    options.UseSqlServer(connectionString);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Proveedor '{proveedor}' no soportado. Usa 'SqlServer'.");
            }
        });

        return services;
    }
}