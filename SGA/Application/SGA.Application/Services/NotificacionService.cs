using SGA.Application.DTOs.Notificaciones;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Error;
using SGA.Domain.Models.Notificaciones;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Validation;

namespace SGA.Application.Services
{
    public sealed class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;

        public NotificacionService(INotificacionRepository notificacionRepository)
            => _notificacionRepository = notificacionRepository;

        public async Task<Result<IReadOnlyList<NotificacionDto>>> ListarPorUsuarioAsync(int usuarioId)
        {
            var validacion = ValidationGeneral.IdValido(usuarioId, "usuario");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<NotificacionDto>>.Fallo(validacion.Error!);
            }

            var notificaciones = await _notificacionRepository.GetByUsuario(usuarioId);
            return Result<IReadOnlyList<NotificacionDto>>.Ok(notificaciones.Select(Mapear).ToList());
        }

        public async Task<Result<IReadOnlyList<NotificacionDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta)
        {
            var validacion = ValidationGeneral.RangoFechasValido(desde, hasta, "notificaciones");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<NotificacionDto>>.Fallo(validacion.Error!);
            }

            var notificaciones = await _notificacionRepository.GetByPeriodo(desde, hasta);
            return Result<IReadOnlyList<NotificacionDto>>.Ok(notificaciones.Select(Mapear).ToList());
        }

        public async Task<Result> MarcarComoLeidaAsync(int notificacionId)
        {
            var validacion = ValidationGeneral.IdValido(notificacionId, "notificacion");

            if (validacion.EsFallo)
            {
                return Result.Fallo(validacion.Error!);
            }

            await _notificacionRepository.MarcarComoLeida(notificacionId);
            return Result.Ok();
        }

        private static NotificacionDto Mapear(NotificacionModel m) =>
            new(m.Id, m.UsuarioTransporteId, m.Tipo, m.Titulo, m.Mensaje, m.FechaHora, m.Leida);
    }
}
