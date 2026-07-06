using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class ParadaRepository : BaseRepository<Parada, ParadaModel>, IParadaRepository
    {
        public ParadaRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<Parada, ParadaModel>> Proyeccion => p =>
            new ParadaModel { 
                Id = p.Id,
                RutaId = p.RutaId,
                Nombre = p.Nombre,
                Referencia = p.Referencia,
                Orden = p.Orden 
            };

        public async Task<IReadOnlyList<ParadaModel>> GetByRuta(int rutaId) =>
            await Set.AsNoTracking()
            .Where(p => p.RutaId == rutaId)
            .OrderBy(p => p.Orden)
            .Select(Proyeccion).ToListAsync();
    }
}