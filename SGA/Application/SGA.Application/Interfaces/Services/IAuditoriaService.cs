using SGA.Application.DTOs.Auditoria;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IAuditoriaService
    {
        Task<Result<IReadOnlyList<AuditoriaDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta);
        Task<Result<IReadOnlyList<AuditoriaDto>>> ListarPorActorAsync(int usuarioId);
        Task<Result<IReadOnlyList<AuditoriaDto>>> ListarPorAccionAsync(string accion);
    }
}