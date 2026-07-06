using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Models.Auditoria;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class AuditoriaRepository : BaseRepository<RegistroAuditoria, AuditoriaModel>, IAuditoriaRepository
    {
        public AuditoriaRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<RegistroAuditoria, AuditoriaModel>> Proyeccion => a => new AuditoriaModel
        {
            Id = a.Id,
            UsuarioTransporteId = a.UsuarioTransporteId,
            Accion = a.Accion,
            EntidadAfectada = a.EntidadAfectada,
            EntidadId = a.EntidadId,
            Detalle = a.Detalle,
            FechaHora = a.FechaHora,
            UsuarioNombre = a.Usuario != null ? a.Usuario.Nombre + " " + a.Usuario.Apellido : null
        };

        public async Task<IReadOnlyList<AuditoriaModel>> GetbyPeriodo(DateTime desde, DateTime hasta) =>
            await Set.AsNoTracking()
            .Include(a => a.Usuario)
            .Where(a => a.FechaHora >= desde && a.FechaHora <= hasta)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<AuditoriaModel>> GetByActor(int usuarioId) =>
            await Set.AsNoTracking().Include(a => a.Usuario)
            .Where(a => a.UsuarioTransporteId == usuarioId)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<AuditoriaModel>> GetbyAccion(string accion) =>
            await Set.AsNoTracking()
            .Include(a => a.Usuario)
            .Where(a => a.Accion == accion)
            .Select(Proyeccion).ToListAsync();
    }
}
