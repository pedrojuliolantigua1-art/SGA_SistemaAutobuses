using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Notificaciones;
using SGA.Domain.Models.Notificaciones;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class NotificacionRepository : BaseRepository<Notificacion, NotificacionModel>, INotificacionRepository
    {
        public NotificacionRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<Notificacion, NotificacionModel>> Proyeccion => n => new NotificacionModel
        {
            Id = n.Id,
            UsuarioTransporteId = n.UsuarioTransporteId,
            Tipo = n.Tipo,
            Titulo = n.Titulo,
            Mensaje = n.Mensaje,
            FechaHora = n.FechaHora,
            Leida = n.Leida,
            UsuarioNombre = n.Usuario != null ? n.Usuario.Nombre + " " + n.Usuario.Apellido : null
        };

        public async Task<IReadOnlyList<NotificacionModel>> GetByUsuario(int usuarioId) =>
            await Set.AsNoTracking().Include(n => n.Usuario)
            .Where(n => n.UsuarioTransporteId == usuarioId)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<NotificacionModel>> GetByPeriodo(DateTime desde, DateTime hasta) =>
            await Set.AsNoTracking().Include(n => n.Usuario)
            .Where(n => n.FechaHora >= desde && n.FechaHora <= hasta)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<NotificacionModel>> GetByTipo(string tipo) =>
            await Set.AsNoTracking().Include(n => n.Usuario)
            .Where(n => n.Tipo == tipo).Select(Proyeccion).ToListAsync();

        public async Task MarcarComoLeida(int notificacionId)
        {
            var notificacion = await Set.FirstAsync(n => n.Id == notificacionId);
            notificacion.Leida = true;
            Set.Update(notificacion);
            await Context.SaveChangesAsync();
        }
    }
}
