using SGA.Domain.Entities.Notificaciones;
using SGA.Domain.Models.Notificaciones;

namespace SGA.Domain.Repository.Interfaces
{
    public interface INotificacionRepository : IBaseRepository<Notificacion, NotificacionModel>
    {
        Task<IReadOnlyList<NotificacionModel>> GetByUsuario(int usuarioId);
        Task<IReadOnlyList<NotificacionModel>> GetByPeriodo(DateTime desde, DateTime hasta);
        Task<IReadOnlyList<NotificacionModel>> GetByTipo(string tipo);
        Task MarcarComoLeida(int notificacionId);
    }
}
