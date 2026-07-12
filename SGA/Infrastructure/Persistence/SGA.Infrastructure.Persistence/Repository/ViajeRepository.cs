using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Entities.Fotos;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Models.Fotos;
using SGA.Domain.Models.Viajes;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Data;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class ViajeRepository : IViajeRepository
    {
        private readonly SgaDbContext _context;
        public ViajeRepository(SgaDbContext context) => _context = context;

        private IQueryable<Viaje> Set => _context.Set<Viaje>();

        private IQueryable<ViajeModel> Consulta =>
            from v in Set.AsNoTracking()
            join c in _context.Set<UsuarioTransporte>() on v.ConductorId equals c.Id into conductorJoin
            from c in conductorJoin.DefaultIfEmpty()
            select new ViajeModel
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
                ConductorNombre = c != null ? c.Nombre + " " + c.Apellido : null,
                Incidencias = (from i in _context.Set<Incidencia>()
                               join ic in _context.Set<UsuarioTransporte>() on i.ConductorId equals ic.Id into incConductorJoin
                               from ic in incConductorJoin.DefaultIfEmpty()
                               where i.ViajeId == v.Id
                               select new IncidenciaModel
                               {
                                   Id = i.Id, ViajeId = i.ViajeId,
                                   ConductorId = i.ConductorId,
                                   Tipo = i.Tipo, Descripcion = i.Descripcion,
                                   FechaHora = i.FechaHora,
                                   ConductorNombre = ic != null ? ic.Nombre + " " + ic.Apellido : null,
                                   Fotos = (from f in _context.Set<FotoIncidencia>()
                                            where f.IncidenciaId == i.Id
                                            select new FotoIncidenciaModel
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

        public async Task<ViajeModel?> GetByIdAsync(int id) =>
            await Consulta.Where(v => v.Id == id).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetAllAsync() =>
            await Consulta.ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyFecha(DateTime fecha) =>
            await Consulta.Where(v => v.Fecha.Date == fecha.Date).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyConductor(int conductorId) =>
            await Consulta.Where(v => v.ConductorId == conductorId).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyAutobusActivo(int autobusId) =>
            await Consulta.Where(v => v.AutobusId == autobusId && v.Estado != EstadoViaje.Finalizado && v.Estado != EstadoViaje.Cancelado).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyPeriodo(DateTime desde, DateTime hasta) =>
            await Consulta.Where(v => v.Fecha >= desde && v.Fecha <= hasta).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetActivos() =>
            await Consulta.Where(v => v.Estado == EstadoViaje.EnCurso).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetProgramados() =>
            await Consulta.Where(v => v.Estado == EstadoViaje.Programado).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyRuta(int rutaId) =>
            await Consulta.Where(v => v.RutaId == rutaId).ToListAsync();

        public async Task<IReadOnlyList<ViajeModel>> GetbyAutobus(int autobusId) =>
            await Consulta.Where(v => v.AutobusId == autobusId).ToListAsync();

        public async Task AddAsync(Viaje entity)
        {
            await _context.Set<Viaje>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Viaje entity)
        {
            _context.Set<Viaje>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Viaje entity)
        {
            entity.Eliminado = true;
            entity.FechaEliminacion = DateTime.Now;
            _context.Attach(entity);
            _context.Entry(entity).Property(e => e.Eliminado).IsModified = true;
            _context.Entry(entity).Property(e => e.FechaEliminacion).IsModified = true;
            _context.Entry(entity).Property(e => e.EliminadoPor).IsModified = true;
            await _context.SaveChangesAsync();
        }

        public async Task AddIncidencia(Incidencia incidencia)
        {
            await _context.Incidencias.AddAsync(incidencia);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<IncidenciaModel>> GetIncidenciasbyPeriodo(DateTime desde, DateTime hasta) =>
            await (from i in _context.Incidencias.AsNoTracking()
                   join ic in _context.Set<UsuarioTransporte>() on i.ConductorId equals ic.Id into incConductorJoin
                   from ic in incConductorJoin.DefaultIfEmpty()
                   where i.FechaHora >= desde && i.FechaHora <= hasta
                   select new IncidenciaModel
                   {
                       Id = i.Id, ViajeId = i.ViajeId,
                       ConductorId = i.ConductorId,
                       Tipo = i.Tipo,
                       Descripcion = i.Descripcion,
                       FechaHora = i.FechaHora,
                       ConductorNombre = ic != null ? ic.Nombre + " " + ic.Apellido : null,
                       Fotos = (from f in _context.Set<FotoIncidencia>()
                                where f.IncidenciaId == i.Id
                                select new FotoIncidenciaModel
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
            await using var transaccion = await _context.Database.BeginTransactionAsync();
            var viaje = await Set.FirstAsync(v => v.Id == viajeId);
            viaje.Estado = EstadoViaje.Cancelado;
            _context.Set<Viaje>().Update(viaje);

            var incidencia = new Incidencia { 
                ViajeId = viajeId, 
                ConductorId = conductorId, 
                Tipo = "CancelacionViaje", 
                Descripcion = motivo, 
                FechaHora = fechaHora, 
                CreadoPor = creadoPor };
            await _context.Incidencias.AddAsync(incidencia);
            await _context.SaveChangesAsync();

            var auditoria = new RegistroAuditoria { 
                UsuarioTransporteId = canceladoPorId,
                Accion = "CancelarViaje",
                EntidadAfectada = nameof(Viaje),
                EntidadId = viajeId.ToString(), 
                Detalle = motivo, FechaHora = fechaHora, 
                CreadoPor = creadoPor };
            await _context.RegistrosAuditoria.AddAsync(auditoria);
            await _context.SaveChangesAsync();

            await transaccion.CommitAsync();
            return incidencia.Id;
        }
    }
}
