using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Enum;
using SGA.Domain.Models.Pagos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Data;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class PagoRepository : IPagoRepository
    {
        private readonly SgaDbContext _context;
        public PagoRepository(SgaDbContext context) => _context = context;

        private IQueryable<PagoTransporte> Set => _context.Set<PagoTransporte>();

        private IQueryable<PagoModel> Consulta =>
            from p in Set.AsNoTracking()
            join u in _context.Set<UsuarioTransporte>() on p.UsuarioTransporteId equals u.Id into usuarioJoin
            from u in usuarioJoin.DefaultIfEmpty()
            join r in _context.Set<UsuarioTransporte>() on p.RegistradoPorUsuarioId equals r.Id into registradorJoin
            from r in registradorJoin.DefaultIfEmpty()
            select new PagoModel
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
                UsuarioNombre = u != null ? u.Nombre + " " + u.Apellido : null,
                RegistradoPorNombre = r != null ? r.Nombre + " " + r.Apellido : null
            };

        public async Task<PagoModel?> GetByIdAsync(int id) =>
            await Consulta.Where(p => p.Id == id).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<PagoModel>> GetAllAsync() =>
            await Consulta.ToListAsync();

        public async Task<IReadOnlyList<PagoModel>> GetByUsuario(int usuarioId) =>
            await Consulta.Where(p => p.UsuarioTransporteId == usuarioId).ToListAsync();

        public async Task<PagoModel?> GetPagoSinAutorizacion(int usuarioId) =>
            await Consulta
            .Where(p => p.UsuarioTransporteId == usuarioId && p.Estado == EstadoPago.Registrado && p.AutorizacionTransporteId == 0)
            .FirstOrDefaultAsync();

        public async Task<IReadOnlyList<PagoModel>> GetByPeriodo(DateTime desde, DateTime hasta) =>
            await Consulta.Where(p => p.FechaHora >= desde && p.FechaHora <= hasta).ToListAsync();

        public async Task AddAsync(PagoTransporte entity)
        {
            await _context.Set<PagoTransporte>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PagoTransporte entity)
        {
            _context.Set<PagoTransporte>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PagoTransporte entity)
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
