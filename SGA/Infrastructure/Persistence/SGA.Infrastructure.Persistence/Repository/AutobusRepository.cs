using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Fotos;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class AutobusRepository : BaseRepository<Autobus, AutobusModel>, IAutobusRepository
    {
        public AutobusRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<Autobus, AutobusModel>> Proyeccion => a =>
            new AutobusModel
            {
                Id = a.Id, Placa = a.Placa,
                Modelo = a.Modelo,
                Capacidad = a.Capacidad,
                Estado = a.Estado,
                Fotos = a.Fotos.Select(f => new FotoAutobusModel
                {
                    Id = f.Id, AutobusId = f.AutobusId,
                    NombreArchivo = f.NombreArchivo,
                    UrlPublica = f.UrlPublica,
                    PublicId = f.PublicId,
                    SubidoPor = f.SubidoPor,
                    FechaSubida = f.FechaSubida
                }).ToList()
            };

        public override async Task<AutobusModel?> GetByIdAsync(int id) =>
            await Set.AsNoTracking().Include(a => a.Fotos)
            .Where(a => a.Id == id)
            .Select(Proyeccion).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<AutobusModel>> GetDisponibles() =>
            await Set.AsNoTracking().Include(a => a.Fotos)
            .Where(a => a.Estado == "Disponible")
            .Select(Proyeccion).ToListAsync();

        public async Task<AutobusModel?> GetByPlaca(string placa) =>
            await Set.AsNoTracking().Include(a => a.Fotos)
            .Where(a => a.Placa == placa)
            .Select(Proyeccion).FirstOrDefaultAsync();
    }
}
