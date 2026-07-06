
using SGA.Application.DTOs.Autobuses;
using SGA.Application.DTOs.Common;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IAutobusService
    {
        Task<Result<IReadOnlyList<AutobusDto>>> ListarTodosAsync();
        Task<Result<IReadOnlyList<AutobusDto>>> ListarDisponiblesAsync();

        Task<Result<AutobusDto>> ObtenerPorIdAsync(int autobusId);

        Task<Result<AutobusDto>> ObtenerPorPlacaAsync(string placa);
        Task<Result<AutobusDto>> CrearAsync(CrearAutobusDto dto);
        Task<Result<AutobusDto>> ActualizarAsync(int autobusId, ActualizarAutobusDto dto);
        Task<Result<AutobusDto>> CambiarEstadoAsync(int autobusId, CambiarEstadoAutobusDto dto);
        Task<Result> EliminarAsync(int autobusId, EliminarDto dto);
        Task<Result> RestaurarAsync(int autobusId, RestaurarDto dto);
    }
}