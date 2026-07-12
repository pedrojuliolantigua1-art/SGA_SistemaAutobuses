using Microsoft.AspNetCore.Mvc;
using SGA.Domain.Error;

namespace SGA.Api.Common
{
    /// <summary>
    /// Convierte los resultados de la capa de dominio en respuestas HTTP apropiadas.
    /// Exito 200 OK, 201 Created, 204 No Content
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

            // Reglas de dominio incumplidas
            return StatusCodes.Status409Conflict;
        }
    }
}
