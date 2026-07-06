using SGA.Application.DTOs.Accesos;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IAccesoService
    {
        // registra un intento de acceso
        Task<Result<AccesoDto>> RegistrarAccesoAsync(RegistrarAccesoDto dto);

        // lista accesos por usuario
        Task<Result<IReadOnlyList<AccesoDto>>> ListarPorUsuarioAsync(int usuarioId);

        Task<Result<AccesoDto>> ObtenerPorIdAsync(int accesoId);

        // lista accesos por viaje
        Task<Result<IReadOnlyList<AccesoDto>>> ListarPorViajeAsync(int viajeId);

        // lista accesos por periodo
        Task<Result<IReadOnlyList<AccesoDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta);
    }
}
