
using SGA.Application.DTOs.Notificaciones;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface INotificacionService
    {
        Task<Result<IReadOnlyList<NotificacionDto>>> ListarPorUsuarioAsync(int usuarioId);
        Task<Result<IReadOnlyList<NotificacionDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta);
        Task<Result> MarcarComoLeidaAsync(int notificacionId);
    }
}