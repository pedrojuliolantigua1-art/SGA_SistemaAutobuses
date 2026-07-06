using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Accesos;
using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Enum;
using SGA.Domain.Models.Accesos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class AccesoRepository : BaseRepository<RegistroUsoTransporte, AccesoModel>, IAccesoRepository
    {
        public AccesoRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<RegistroUsoTransporte, AccesoModel>> Proyeccion => a => new AccesoModel
        {
            Id = a.Id,
            UsuarioTransporteId = a.UsuarioTransporteId,
            ViajeId = a.ViajeId,
            AutorizacionTransporteId = a.AutorizacionTransporteId,
            ResultadoAcceso = a.ResultadoAcceso,
            MotivoRechazo = a.MotivoRechazo,
            FechaHora = a.FechaHora, 
            ValidadoPorUsuarioId = a.ValidadoPorUsuarioId,
            UsuarioNombre = a.Usuario != null ? a.Usuario.Nombre + " " + a.Usuario.Apellido : null,
            ValidadorNombre = a.ValidadoPor != null ? a.ValidadoPor.Nombre + " " + a.ValidadoPor.Apellido : null
        };

        public async Task<IReadOnlyList<AccesoModel>> GetByViaje(int viajeId) =>
            await Set.AsNoTracking()
            .Include(a => a.Usuario)
            .Include(a => a.ValidadoPor)
            .Where(a => a.ViajeId == viajeId).Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<AccesoModel>> GetByUsuario(int usuarioId) =>
            await Set.AsNoTracking()
            .Include(a => a.Usuario)
            .Include(a => a.ValidadoPor)
            .Where(a => a.UsuarioTransporteId == usuarioId).Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<AccesoModel>> GetByPeriodo(DateTime desde, DateTime hasta) =>
            await Set.AsNoTracking()
            .Include(a => a.Usuario)
            .Include(a => a.ValidadoPor)
            .Where(a => a.FechaHora >= desde && a.FechaHora <= hasta)
            .Select(Proyeccion).ToListAsync();

        public async Task<int> RegistrarAbordajeAsync(
            int usuarioId, int viajeId, int autorizacionId, int resultadoAcceso,
            string? motivoRechazo, DateTime fechaHora, int validadorId, string creadoPor, decimal costoViaje = 30.00m)
        {
            await using var transaccion = await Context.Database.BeginTransactionAsync();
            var acceso = new RegistroUsoTransporte
            {
                UsuarioTransporteId = usuarioId, ViajeId = viajeId,
                AutorizacionTransporteId = autorizacionId == 0 ? null : autorizacionId,
                ResultadoAcceso = (ResultadoAcceso)resultadoAcceso, MotivoRechazo = motivoRechazo,
                FechaHora = fechaHora, ValidadoPorUsuarioId = validadorId, CreadoPor = creadoPor
            };
            await Set.AddAsync(acceso);
            await Context.SaveChangesAsync();

            if (acceso.ResultadoAcceso == ResultadoAcceso.Permitido && acceso.AutorizacionTransporteId is int autId)
            {
                var tarjeta = await Context.TarjetasRecargables.FirstOrDefaultAsync(t => t.Id == autId);
                if (tarjeta is not null) { tarjeta.SaldoDisponible -= costoViaje; Context.TarjetasRecargables.Update(tarjeta); await Context.SaveChangesAsync(); }
            }

            await Context.RegistrosAuditoria.AddAsync(new RegistroAuditoria
            {
                UsuarioTransporteId = validadorId, Accion = "RegistrarAcceso",
                EntidadAfectada = nameof(RegistroUsoTransporte), EntidadId = acceso.Id.ToString(),
                Detalle = $"Resultado: {acceso.ResultadoAcceso}", FechaHora = fechaHora, CreadoPor = creadoPor
            });
            await Context.SaveChangesAsync();
            await transaccion.CommitAsync();
            return acceso.Id;
        }
    }
}
