using SGA.Domain.Entities.Notificaciones;
using SGA.Domain.Models.Notificaciones;

namespace SGA.Domain.Repository.Interfaces
{
    public interface INotificacionRepository : IBaseRepository<Notificacion, NotificacionModel>
    {
        /// <summary>
        /// Obtiene todas las notificaciones de un usuario específico.
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<NotificacionModel>> GetByUsuario(int usuarioId);

        /// <summary>
        /// Para obtener todas las notificaciones de un usuario específico en un rango de fechas.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<NotificacionModel>> GetByPeriodo(DateTime desde, DateTime hasta);

        /// <summary>
        /// Obtiene todas las notificaciones de un usuario específico por tipo.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        Task<IReadOnlyList<NotificacionModel>> GetByTipo(string tipo);

        /// <summary>
        /// Este metodo se encarga de marcar una notificación como leída en la base de datos, 
        /// actualizando su estado para reflejar que el usuario ha visto la notificación.
        /// </summary>
        /// <param name="notificacionId"></param>
        /// <returns></returns>
        Task MarcarComoLeida(int notificacionId);
    }
}
