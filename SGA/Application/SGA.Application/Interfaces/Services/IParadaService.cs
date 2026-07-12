

using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Paradas;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IParadaService
    {
        Task<Result<IReadOnlyList<ParadaDto>>> ListarPorRutaAsync(int rutaId);

        Task<Result<ParadaDto>> ObtenerPorIdAsync(int paradaId);
        Task<Result<ParadaDto>> CrearAsync(CrearParadaDto dto);
        Task<Result<ParadaDto>> ActualizarAsync(int paradaId, ActualizarParadaDto dto);
        Task<Result> ReordenarAsync(ReordenarParadasDto dto);
        Task<Result> EliminarAsync(int paradaId, EliminarDto dto);
    }
}