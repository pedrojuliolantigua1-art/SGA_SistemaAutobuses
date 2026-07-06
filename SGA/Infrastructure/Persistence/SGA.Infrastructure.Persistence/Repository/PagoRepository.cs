using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Models.Pagos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class PagoRepository : BaseRepository<PagoTransporte, PagoModel>, IPagoRepository
    {
        public PagoRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<PagoTransporte, PagoModel>> Proyeccion => p => new PagoModel
        {
            Id = p.Id,
            UsuarioTransporteId = p.UsuarioTransporteId,
            AutorizacionTransporteId = p.AutorizacionTransporteId,
            Monto = p.Monto,
            TipoPago = p.TipoPago,
            Estado = p.Estado,
            NumeroComprobante = p.NumeroComprobante,
            FechaHora = p.FechaHora,
            RegistradoPorUsuarioId = p.RegistradoPorUsuarioId,
            UsuarioNombre = p.Usuario != null ? p.Usuario.Nombre + " " + p.Usuario.Apellido : null,
            RegistradoPorNombre = p.RegistradoPor != null ? p.RegistradoPor.Nombre + " " + p.RegistradoPor.Apellido : null
        };

        public async Task<IReadOnlyList<PagoModel>> GetByUsuario(int usuarioId) =>
            await Set.AsNoTracking()
            .Include(p => p.Usuario)
            .Include(p => p.RegistradoPor)
            .Where(p => p.UsuarioTransporteId == usuarioId)
            .Select(Proyeccion).ToListAsync();

        public async Task<PagoModel?> GetPagoSinAutorizacion(int usuarioId) =>
            await Set.AsNoTracking()
            .Where(p => p.UsuarioTransporteId == usuarioId && p.AutorizacionTransporteId == 0)
            .Select(Proyeccion).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<PagoModel>> GetByPeriodo(DateTime desde, DateTime hasta) =>
            await Set.AsNoTracking().Include(p => p.Usuario).Include(p => p.RegistradoPor)
            .Where(p => p.FechaHora >= desde && p.FechaHora <= hasta)
            .Select(Proyeccion).ToListAsync();
    }
}
