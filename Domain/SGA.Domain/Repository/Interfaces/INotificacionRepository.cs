using SGA.Domain.Entities.Notificaciones;

namespace SGA.Domain.Repository.Interfaces
{
    public interface INotificacionRepository : IBaseRepository<Notificacion>
    {
        Task<IReadOnlyList<Notificacion>> GetBy_Usuario(int usuarioId);
        Task<IReadOnlyList<Notificacion>> GetBy_Periodo(DateTime desde, DateTime hasta);
        Task<IReadOnlyList<Notificacion>> GetBy_Tipo(string tipo);
        Task MarcarComoLeida(int notificacionId);
    }
}
