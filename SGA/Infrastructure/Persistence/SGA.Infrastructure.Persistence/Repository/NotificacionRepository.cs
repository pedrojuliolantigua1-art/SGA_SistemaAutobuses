using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Notificaciones;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Models.Notificaciones;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Data;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class NotificacionRepository : INotificacionRepository
    {
        private readonly SgaDbContext _context;
        public NotificacionRepository(SgaDbContext context) => _context = context;

        private IQueryable<Notificacion> Set => _context.Set<Notificacion>();

        private IQueryable<NotificacionModel> Consulta =>
            from n in Set.AsNoTracking()
            join u in _context.Set<UsuarioTransporte>() on n.UsuarioTransporteId equals u.Id into usuarioJoin
            from u in usuarioJoin.DefaultIfEmpty()
            select new NotificacionModel
            {
                Id = n.Id,
                UsuarioTransporteId = n.UsuarioTransporteId,
                Tipo = n.Tipo,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                FechaHora = n.FechaHora,
                Leida = n.Leida,
                UsuarioNombre = u != null ? u.Nombre + " " + u.Apellido : null
            };

        public async Task<NotificacionModel?> GetByIdAsync(int id) =>
            await Consulta.Where(n => n.Id == id).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<NotificacionModel>> GetAllAsync() =>
            await Consulta.ToListAsync();

        public async Task<IReadOnlyList<NotificacionModel>> GetByUsuario(int usuarioId) =>
            await Consulta.Where(n => n.UsuarioTransporteId == usuarioId).ToListAsync();

        public async Task<IReadOnlyList<NotificacionModel>> GetByPeriodo(DateTime desde, DateTime hasta) =>
            await Consulta.Where(n => n.FechaHora >= desde && n.FechaHora <= hasta).ToListAsync();

        public async Task<IReadOnlyList<NotificacionModel>> GetByTipo(string tipo) =>
            await Consulta.Where(n => n.Tipo == tipo).ToListAsync();

        public async Task MarcarComoLeida(int notificacionId)
        {
            var notificacion = await Set.FirstAsync(n => n.Id == notificacionId);
            notificacion.Leida = true;
            _context.Set<Notificacion>().Update(notificacion);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Notificacion entity)
        {
            await _context.Set<Notificacion>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notificacion entity)
        {
            _context.Set<Notificacion>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Notificacion entity)
        {
            entity.Eliminado = true;
            entity.FechaEliminacion = DateTime.Now;
            _context.Attach(entity);
            _context.Entry(entity).Property(e => e.Eliminado).IsModified = true;
            _context.Entry(entity).Property(e => e.FechaEliminacion).IsModified = true;
            _context.Entry(entity).Property(e => e.EliminadoPor).IsModified = true;
            await _context.SaveChangesAsync();
        }
    }
}
