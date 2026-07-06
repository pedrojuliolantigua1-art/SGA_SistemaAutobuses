using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Fotos;
using SGA.Domain.Models.Fotos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class FotoAutobusRepository : BaseRepository<FotoAutobus, FotoAutobusModel>, IFotoAutobusRepository
    {
        public FotoAutobusRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<FotoAutobus, FotoAutobusModel>> Proyeccion => f => new FotoAutobusModel
        {
            Id = f.Id,
            AutobusId = f.AutobusId,
            NombreArchivo = f.NombreArchivo,
            UrlPublica = f.UrlPublica,
            PublicId = f.PublicId,
            SubidoPor = f.SubidoPor,
            FechaSubida = f.FechaSubida
        };

        public async Task<FotoAutobusModel?> GetByAutobusId(int autobusId) =>
            await Set.AsNoTracking()
            .Where(f => f.AutobusId == autobusId)
            .Select(Proyeccion)
            .FirstOrDefaultAsync();

        public async Task<IReadOnlyList<FotoAutobusModel>> GetAllByAutobusId(int autobusId) =>
            await Set.AsNoTracking()
            .Where(f => f.AutobusId == autobusId)
            .Select(Proyeccion)
            .ToListAsync();
    }
}