using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class HorarioRutaRepository : BaseRepository<HorarioRuta, HorarioModel>, IHorarioRutaRepository
    {
        public HorarioRutaRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<HorarioRuta, HorarioModel>> Proyeccion => h =>
            new HorarioModel { 
                Id = h.Id,
                RutaId = h.RutaId,
                HoraSalida = h.HoraSalida,
                HoraLlegadaEstimada = h.HoraLlegadaEstimada,
                Activo = h.Activo 
            };

        public async Task<IReadOnlyList<HorarioModel>> GetByRuta(int rutaId) =>
            await Set.AsNoTracking()
            .Where(h => h.RutaId == rutaId && h.Activo)
            .Select(Proyeccion)
            .ToListAsync();
    }
}