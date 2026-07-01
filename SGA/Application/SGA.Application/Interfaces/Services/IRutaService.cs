using SGA.Application.DTOs.Rutas;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IRutaService
    {
        // lista todas las rutas registradas
        Task<Result<IReadOnlyList<RutaDto>>> ListarTodasAsync();

        // lista solo las rutas activas
        Task<Result<IReadOnlyList<RutaDto>>> ListarActivasAsync();

        // obtiene una ruta con sus paradas y horarios
        Task<Result<RutaDetalleDto>> ObtenerDetalleAsync(int rutaId);

        // crea una ruta nueva
        Task<Result<RutaDto>> CrearAsync(CrearRutaDto dto);

        // actualiza una ruta existente
        Task<Result<RutaDto>> ActualizarAsync(ActualizarRutaDto dto);

        // elimina una ruta de forma logica
        Task<Result> EliminarAsync(int rutaId, string? eliminadoPor);
    }
}
