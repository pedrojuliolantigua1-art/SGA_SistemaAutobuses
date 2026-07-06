using Microsoft.AspNetCore.Mvc;
using SGA.Domain.Error;

namespace SGA.Api.Common
{
    /// <summary>
    /// Convierte los Result / Result{T} del dominio en respuestas HTTP consistentes.
    /// - Exito                     -> 200 OK (o 201 si se indica)
    /// - Aplicacion.NoEncontrado   -> 404 Not Found
    /// - General.* (validaciones)  -> 400 Bad Request
    /// - Reglas de negocio         -> 409 Conflict
    /// </summary>
    public static class ResultadosHttp
    {
        public static IActionResult AProblema(this ControllerBase controller, Error error)
        {
            var status = ObtenerStatus(error);

            return controller.Problem(
                title: error.Codigo,
                detail: error.Mensaje,
                statusCode: status);
        }

        public static IActionResult AResultado<T>(this ControllerBase controller, Result<T> result)
            => result.EsExitoso
                ? controller.Ok(result.Valor)
                : controller.AProblema(result.Error!);

        public static IActionResult AResultado(this ControllerBase controller, Result result)
            => result.EsExitoso
                ? controller.NoContent()
                : controller.AProblema(result.Error!);

        public static IActionResult AResultadoCreado<T>(
            this ControllerBase controller, Result<T> result, string? location = null)
            => result.EsExitoso
                ? controller.Created(location ?? string.Empty, result.Valor)
                : controller.AProblema(result.Error!);

        private static int ObtenerStatus(Error error)
        {
            if (error.Codigo == "Aplicacion.NoEncontrado")
                return StatusCodes.Status404NotFound;

            if (error.Codigo.StartsWith("General.", StringComparison.Ordinal))
                return StatusCodes.Status400BadRequest;

            // Reglas de dominio incumplidas (Acceso.*, Autorizacion.*, Viaje.*, etc.)
            return StatusCodes.Status409Conflict;
        }
    }
}
