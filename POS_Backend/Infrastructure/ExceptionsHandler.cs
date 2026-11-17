using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using POS_Backend.Logica.Exceptions;

namespace POS_Backend.Infraestructura;

public class ManejadorExcepciones : IExceptionHandler
{
    private readonly ILogger<ManejadorExcepciones> _logger;

    public ManejadorExcepciones(ILogger<ManejadorExcepciones> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        ProblemDetails problema;

        switch (exception)
        {
            case ReglaNegocioException regla:
                problema = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "No se cumple una regla de negocio",
                    Detail = regla.Message
                };
                problema.Extensions["errores"] = regla.Errores;
                break;

            case NoEncontradoException:
                problema = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "No encontrado",
                    Detail = exception.Message
                };
                break;

            default:
                _logger.LogError(exception, "Error no controlado");
                problema = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Error interno",
                    Detail = "Ocurrió un error inesperado."
                };
                break;
        }

        httpContext.Response.StatusCode = problema.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problema, ct);
        return true;
    }
}