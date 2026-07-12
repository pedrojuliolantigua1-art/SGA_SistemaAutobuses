
using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Horarios;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IHorarioRutaService
    {
        Task<Result<IReadOnlyList<HorarioRutaDto>>> ListarPorRutaAsync(int rutaId);

        Task<Result<HorarioRutaDto>> ObtenerPorIdAsync(int horarioId);
        Task<Result<HorarioRutaDto>> CrearAsync(CrearHorarioRutaDto dto);
        Task<Result<HorarioRutaDto>> ActualizarAsync(int horarioId, ActualizarHorarioRutaDto dto);
        Task<Result> EliminarAsync(int horarioId, EliminarDto dto);
    }
}