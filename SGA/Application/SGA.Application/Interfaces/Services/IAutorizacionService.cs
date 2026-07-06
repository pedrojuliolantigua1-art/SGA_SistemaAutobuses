using SGA.Application.DTOs.Autorizaciones;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IAutorizacionService
    {
        Task<Result<AutorizacionResumenDto>> ObtenerPorUsuarioAsync(int usuarioId);

        Task<Result<AutorizacionResumenDto>> ObtenerPorIdAsync(int autorizacionId);
        Task<Result<IReadOnlyList<AutorizacionResumenDto>>> ListarVigentesAsync();
        Task<Result<IReadOnlyList<AutorizacionResumenDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta);

        Task<Result<TicketDiarioDto>> EmitirTicketDiarioAsync(CrearTicketDiarioDto dto);
        Task<Result<TarjetaRecargableDto>> EmitirTarjetaRecargableAsync(CrearTarjetaRecargableDto dto);
        Task<Result<PermisoTransporteDto>> EmitirPermisoAsync(CrearPermisoTransporteDto dto);

        Task<Result> AnularAsync(int autorizacionId, AnularAutorizacionDto dto);
    }
}
