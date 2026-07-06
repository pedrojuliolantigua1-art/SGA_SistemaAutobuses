using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Models.Fotos;
using SGA.Domain.Models.Viajes;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class ViajeRepository : BaseRepository<Viaje, ViajeModel>, IViajeRepository
    {
        public ViajeRepository(SgaDbContext context) : base(context) { }

        protected override Expression<Func<Viaje, ViajeModel>> Proyeccion => v => new ViajeModel
        {
            Id = v.Id, RutaId = v.RutaId,
            HorarioRutaId = v.HorarioRutaId,
            AutobusId = v.AutobusId,
            ConductorId = v.ConductorId,
            Fecha = v.Fecha,
            Estado = v.Estado,
            HoraInicioReal = v.HoraInicioReal,
            HoraFinReal = v.HoraFinReal,
            CupoActual = v.CupoActual,
            CapacidadMaxima = v.CapacidadMaxima,
            ConductorNombre = v.Conductor != null ? v.Conductor.Nombre + " " + v.Conductor.Apellido : null,
            Incidencias = v.Incidencias.Select(i => new IncidenciaModel
            {
                Id = i.Id, ViajeId = i.ViajeId,
                ConductorId = i.ConductorId,
                Tipo = i.Tipo, Descripcion = i.Descripcion,
                FechaHora = i.FechaHora,
                ConductorNombre = i.Conductor != null ? i.Conductor.Nombre + " " + i.Conductor.Apellido : null,
                Fotos = i.Fotos.Select(f => new FotoIncidenciaModel
                {
                    Id = f.Id, IncidenciaId = f.IncidenciaId,
                    NombreArchivo = f.NombreArchivo,
                    UrlPublica = f.UrlPublica,
                    PublicId = f.PublicId,
                    SubidoPor = f.SubidoPor,
                    FechaSubida = f.FechaSubida
                }).ToList()
            }).ToList()
        };

        public override async Task<ViajeModel?> GetByIdAsync(int id) =>
            await Set.AsNoTracking()
                .Include(v => v.Conductor)
                .Include(v => v.Incidencias).ThenInclude(i => i.Conductor)
                .Include(v => v.Incidencias).ThenInclude(i => i.Fotos)
                .Where(v => v.Id == id).Select(Proyeccion).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyFecha(DateTime fecha) =>
            await Set.AsNoTracking()
            .Include(v => v.Conductor)
            .Include(v => v.Incidencias)
            .Where(v => v.Fecha.Date == fecha.Date)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyConductor(int conductorId) =>
            await Set.AsNoTracking()
            .Include(v => v.Conductor)
            .Include(v => v.Incidencias)
            .Where(v => v.ConductorId == conductorId)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyAutobusActivo(int autobusId) =>
            await Set.AsNoTracking()
            .Where(v => v.AutobusId == autobusId && v.Estado != EstadoViaje.Finalizado && v.Estado != EstadoViaje.Cancelado)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyPeriodo(DateTime desde, DateTime hasta) =>
            await Set.AsNoTracking()
            .Include(v => v.Conductor)
            .Include(v => v.Incidencias)
            .Where(v => v.Fecha >= desde && v.Fecha <= hasta)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetActivos() =>
            await Set.AsNoTracking()
            .Include(v => v.Conductor)
            .Where(v => v.Estado == EstadoViaje.EnCurso)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetProgramados() =>
            await Set.AsNoTracking()
            .Include(v => v.Conductor)
            .Where(v => v.Estado == EstadoViaje.Programado)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyRuta(int rutaId) =>
            await Set.AsNoTracking()
            .Include(v => v.Conductor)
            .Where(v => v.RutaId == rutaId)
            .Select(Proyeccion).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyAutobus(int autobusId) =>
            await Set.AsNoTracking()
            .Include(v => v.Conductor)
            .Where(v => v.AutobusId == autobusId)
            .Select(Proyeccion).ToListAsync();

        public async Task AddIncidencia(Incidencia incidencia)
        {
            await Context.Incidencias.AddAsync(incidencia);
            await Context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<IncidenciaModel>> GetIncidenciasbyPeriodo(DateTime desde, DateTime hasta) =>
            await Context.Incidencias.AsNoTracking()
                .Include(i => i.Conductor).Include(i => i.Fotos)
                .Where(i => i.FechaHora >= desde && i.FechaHora <= hasta)
                .Select(i => new IncidenciaModel
                {
                    Id = i.Id, ViajeId = i.ViajeId,
                    ConductorId = i.ConductorId,
                    Tipo = i.Tipo,
                    Descripcion = i.Descripcion,
                    FechaHora = i.FechaHora,
                    ConductorNombre = i.Conductor != null ? i.Conductor.Nombre + " " + i.Conductor.Apellido : null,
                    Fotos = i.Fotos.Select(f => new FotoIncidenciaModel
                    {
                        Id = f.Id, IncidenciaId = f.IncidenciaId,
                        NombreArchivo = f.NombreArchivo,
                        UrlPublica = f.UrlPublica,
                        PublicId = f.PublicId,
                        SubidoPor = f.SubidoPor,
                        FechaSubida = f.FechaSubida
                    }).ToList()
                }).ToListAsync();

        public async Task<int> CancelarViajeAsync(
            int viajeId, int conductorId, string motivo,
            DateTime fechaHora, int canceladoPorId, string creadoPor)
        {
            await using var transaccion = await Context.Database.BeginTransactionAsync();
            var viaje = await Set.FirstAsync(v => v.Id == viajeId);
            viaje.Estado = EstadoViaje.Cancelado;
            Set.Update(viaje);

            var incidencia = new Incidencia { 
                ViajeId = viajeId, 
                ConductorId = conductorId, 
                Tipo = "CancelacionViaje", 
                Descripcion = motivo, 
                FechaHora = fechaHora, 
                CreadoPor = creadoPor };
            await Context.Incidencias.AddAsync(incidencia);
            await Context.SaveChangesAsync();

            var auditoria = new RegistroAuditoria { 
                UsuarioTransporteId = canceladoPorId,
                Accion = "CancelarViaje",
                EntidadAfectada = nameof(Viaje),
                EntidadId = viajeId.ToString(), 
                Detalle = motivo, FechaHora = fechaHora, 
                CreadoPor = creadoPor };
            await Context.RegistrosAuditoria.AddAsync(auditoria);
            await Context.SaveChangesAsync();

            await transaccion.CommitAsync();
            return incidencia.Id;
        }
    }
}
