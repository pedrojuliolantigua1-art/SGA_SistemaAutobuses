using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Rutas;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IRutaService
    {
        // lista todas las rutas
        Task<Result<IReadOnlyList<RutaDto>>> ListarTodasAsync();

        // lista solo las rutas activas
        Task<Result<IReadOnlyList<RutaDto>>> ListarActivasAsync();

        // obtiene el detalle de una ruta por su id
        Task<Result<RutaDetalleDto>> ObtenerDetalleAsync(int rutaId);

        Task<Result<RutaDto>> ObtenerPorIdAsync(int rutaId);

        //  obtiene el detalle de una ruta por su id, incluyendo las paradas y horarios
        Task<Result<RutaDto>> CrearAsync(CrearRutaDto dto);

        // actualiza una ruta existente
        Task<Result<RutaDto>> ActualizarAsync(int rutaId, ActualizarRutaDto dto);

        // elimina una ruta existente
        Task<Result> EliminarAsync(int rutaId, EliminarDto dto);

        // restaura una ruta eliminada
        Task<Result> RestaurarAsync(int rutaId, RestaurarDto dto);
    }
}