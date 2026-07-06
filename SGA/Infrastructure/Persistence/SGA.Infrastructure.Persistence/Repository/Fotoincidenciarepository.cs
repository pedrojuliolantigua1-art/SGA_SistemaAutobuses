using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Fotos;
using SGA.Domain.Models.Fotos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class FotoIncidenciaRepository : BaseRepository<FotoIncidencia, FotoIncidenciaModel>, IFotoIncidenciaRepository
    {
        public FotoIncidenciaRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<FotoIncidencia, FotoIncidenciaModel>> Proyeccion => f => new FotoIncidenciaModel
        {
            Id = f.Id,
            IncidenciaId = f.IncidenciaId,
            NombreArchivo = f.NombreArchivo,
            UrlPublica = f.UrlPublica,
            PublicId = f.PublicId,
            SubidoPor = f.SubidoPor,
            FechaSubida = f.FechaSubida
        };

        public async Task<IReadOnlyList<FotoIncidenciaModel>> GetByIncidenciaId(int incidenciaId) =>
            await Set.AsNoTracking()
            .Where(f => f.IncidenciaId == incidenciaId)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<FotoIncidenciaModel>> GetByViajeId(int viajeId) =>
            await Set.AsNoTracking()
                .Include(f => f.Incidencia)
                .Where(f => f.Incidencia != null && f.Incidencia.ViajeId == viajeId)
                .Select(Proyeccion)
                .ToListAsync();
    }
}
