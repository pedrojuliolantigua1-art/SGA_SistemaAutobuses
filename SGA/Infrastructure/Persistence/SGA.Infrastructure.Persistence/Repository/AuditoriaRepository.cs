using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Models.Auditoria;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly SgaDbContext _context;
        public AuditoriaRepository(SgaDbContext context) => _context = context;

        private IQueryable<AuditoriaModel> Consulta =>
            from a in _context.Set<RegistroAuditoria>().AsNoTracking()
            join u in _context.Set<UsuarioTransporte>() on a.UsuarioTransporteId equals u.Id into usuarioJoin
            from u in usuarioJoin.DefaultIfEmpty()
            select new AuditoriaModel
            {
                Id = a.Id,
                UsuarioTransporteId = a.UsuarioTransporteId,
                Accion = a.Accion,
                EntidadAfectada = a.EntidadAfectada,
                EntidadId = a.EntidadId,
                Detalle = a.Detalle,
                FechaHora = a.FechaHora,
                UsuarioNombre = u != null ? u.Nombre + " " + u.Apellido : null
            };

        public async Task<AuditoriaModel?> GetByIdAsync(int id) =>
            await Consulta.Where(a => a.Id == id).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<AuditoriaModel>> GetAllAsync() =>
            await Consulta.ToListAsync();

        public async Task<IReadOnlyList<AuditoriaModel>> GetbyPeriodo(DateTime desde, DateTime hasta) =>
            await Consulta.Where(a => a.FechaHora >= desde && a.FechaHora <= hasta).ToListAsync();

        public async Task<IReadOnlyList<AuditoriaModel>> GetByActor(int usuarioId) =>
            await Consulta.Where(a => a.UsuarioTransporteId == usuarioId).ToListAsync();

        public async Task<IReadOnlyList<AuditoriaModel>> GetbyAccion(string accion) =>
            await Consulta.Where(a => a.Accion == accion).ToListAsync();

        public async Task AddAsync(RegistroAuditoria entity)
        {
            await _context.Set<RegistroAuditoria>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RegistroAuditoria entity)
        {
            _context.Set<RegistroAuditoria>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(RegistroAuditoria entity)
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
