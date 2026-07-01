using SGA.Application.DTOs.Autorizaciones;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IAutorizacionService
    {
        // obtiene la autorizacion activa de un usuario
        Task<Result<AutorizacionDto>> ObtenerPorUsuarioAsync(int usuarioId);

        // lista autorizaciones vigentes
        Task<Result<IReadOnlyList<AutorizacionDto>>> ListarVigentesAsync();

        // lista autorizaciones por periodo
        Task<Result<IReadOnlyList<AutorizacionDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta);

        // emite un ticket mensual
        Task<Result<AutorizacionDto>> EmitirTicketMensualAsync(CrearTicketMensualDto dto);

        // emite una tarjeta recargable
        Task<Result<AutorizacionDto>> EmitirTarjetaRecargableAsync(CrearTarjetaRecargableDto dto);

        // emite un permiso de transporte
        Task<Result<AutorizacionDto>> EmitirPermisoAsync(CrearPermisoTransporteDto dto);

        // anula una autorizacion
        Task<Result> AnularAsync(int autorizacionId);
    }
}
