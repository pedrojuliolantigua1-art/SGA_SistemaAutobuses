using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class RutaRepository : BaseRepository<Ruta, RutaModel>, IRutaRepository
    {
        public RutaRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<Ruta, RutaModel>> Proyeccion => r =>
            new RutaModel { 
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                Activa = r.Activa };

        public async Task<IReadOnlyList<RutaModel>> GetActivas() =>
            await Set.AsNoTracking().Where(r => r.Activa).Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<ParadaModel>> GetParadas(int rutaId) =>
            await (from p in Context.Set<Parada>().AsNoTracking()
                   where p.RutaId == rutaId
                   orderby p.Orden
                   select new ParadaModel
                   {
                       Id = p.Id,
                       RutaId = p.RutaId,
                       Nombre = p.Nombre,
                       Referencia = p.Referencia,
                       Orden = p.Orden
                   }).ToListAsync();

        public async Task<IReadOnlyList<HorarioModel>> GetHorarios(int rutaId) =>
            await (from h in Context.Set<HorarioRuta>().AsNoTracking()
                   where h.RutaId == rutaId && h.Activo
                   select new HorarioModel
                   {
                       Id = h.Id,
                       RutaId = h.RutaId,
                       HoraSalida = h.HoraSalida,
                       HoraLlegadaEstimada = h.HoraLlegadaEstimada,
                       Activo = h.Activo
                   }).ToListAsync();
    }
}
