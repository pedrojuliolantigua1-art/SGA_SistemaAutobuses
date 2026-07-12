using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Accesos;
using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Enum;
using SGA.Domain.Models.Accesos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Data;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class AccesoRepository : IAccesoRepository
    {
        private readonly SgaDbContext _context;
        public AccesoRepository(SgaDbContext context) => _context = context;

        private IQueryable<RegistroUsoTransporte> Set => _context.Set<RegistroUsoTransporte>();

        private IQueryable<AccesoModel> Consulta =>
            from a in Set.AsNoTracking()
            join u in _context.Set<UsuarioTransporte>() on a.UsuarioTransporteId equals u.Id into usuarioJoin
            from u in usuarioJoin.DefaultIfEmpty()
            join v in _context.Set<UsuarioTransporte>() on a.ValidadoPorUsuarioId equals v.Id into validadorJoin
            from v in validadorJoin.DefaultIfEmpty()
            select new AccesoModel
            {
                Id = a.Id,
                UsuarioTransporteId = a.UsuarioTransporteId,
                ViajeId = a.ViajeId,
                AutorizacionTransporteId = a.AutorizacionTransporteId,
                ResultadoAcceso = a.ResultadoAcceso,
                MotivoRechazo = a.MotivoRechazo,
                FechaHora = a.FechaHora,
                ValidadoPorUsuarioId = a.ValidadoPorUsuarioId,
                UsuarioNombre = u != null ? u.Nombre + " " + u.Apellido : null,
                ValidadorNombre = v != null ? v.Nombre + " " + v.Apellido : null
            };

        public async Task<AccesoModel?> GetByIdAsync(int id) =>
            await Consulta.Where(a => a.Id == id).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<AccesoModel>> GetAllAsync() =>
            await Consulta.ToListAsync();

        public async Task<IReadOnlyList<AccesoModel>> GetByViaje(int viajeId) =>
            await Consulta.Where(a => a.ViajeId == viajeId).ToListAsync();

        public async Task<IReadOnlyList<AccesoModel>> GetByUsuario(int usuarioId) =>
            await Consulta.Where(a => a.UsuarioTransporteId == usuarioId).ToListAsync();

        public async Task<IReadOnlyList<AccesoModel>> GetByPeriodo(DateTime desde, DateTime hasta) =>
            await Consulta.Where(a => a.FechaHora >= desde && a.FechaHora <= hasta).ToListAsync();

        public async Task AddAsync(RegistroUsoTransporte entity)
        {
            await _context.Set<RegistroUsoTransporte>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RegistroUsoTransporte entity)
        {
            _context.Set<RegistroUsoTransporte>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(RegistroUsoTransporte entity)
        {
            entity.Eliminado = true;
            entity.FechaEliminacion = DateTime.Now;
            _context.Attach(entity);
            _context.Entry(entity).Property(e => e.Eliminado).IsModified = true;
            _context.Entry(entity).Property(e => e.FechaEliminacion).IsModified = true;
            _context.Entry(entity).Property(e => e.EliminadoPor).IsModified = true;
            await _context.SaveChangesAsync();
        }

        public async Task<int> RegistrarAbordajeAsync(
            int usuarioId, int viajeId, int autorizacionId, int resultadoAcceso,
            string? motivoRechazo, DateTime fechaHora, int validadorId, string creadoPor, decimal costoViaje = 60.00m)
        {
            await using var transaccion = await _context.Database.BeginTransactionAsync();
            var acceso = new RegistroUsoTransporte
            {
                UsuarioTransporteId = usuarioId, ViajeId = viajeId,
                AutorizacionTransporteId = autorizacionId == 0 ? null : autorizacionId,
                ResultadoAcceso = (ResultadoAcceso)resultadoAcceso, MotivoRechazo = motivoRechazo,
                FechaHora = fechaHora, ValidadoPorUsuarioId = validadorId, CreadoPor = creadoPor
            };
            await _context.Set<RegistroUsoTransporte>().AddAsync(acceso);
            await _context.SaveChangesAsync();

            if (acceso.ResultadoAcceso == ResultadoAcceso.Permitido && acceso.AutorizacionTransporteId is int autId)
            {
                var tarjeta = await _context.TarjetasRecargables.FirstOrDefaultAsync(t => t.Id == autId);
                if (tarjeta is not null) { tarjeta.SaldoDisponible -= costoViaje; _context.TarjetasRecargables.Update(tarjeta); await _context.SaveChangesAsync(); }
            }

            await _context.RegistrosAuditoria.AddAsync(new RegistroAuditoria
            {
                UsuarioTransporteId = validadorId, Accion = "RegistrarAcceso",
                EntidadAfectada = nameof(RegistroUsoTransporte), EntidadId = acceso.Id.ToString(),
                Detalle = $"Resultado: {acceso.ResultadoAcceso}", FechaHora = fechaHora, CreadoPor = creadoPor
            });
            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();
            return acceso.Id;
        }
    }
}
