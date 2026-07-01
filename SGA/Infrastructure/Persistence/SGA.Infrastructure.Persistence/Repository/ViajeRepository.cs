using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Models.Viajes;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class ViajeRepository : SqlRepositoryBase, IViajeRepository
    {
        public ViajeRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<ViajeModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_Viaje_GetById", ViajeMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<ViajeModel>> GetAllAsync()
            => await QueryAsync("sp_Viaje_GetAll", ViajeMapper.Map);

        public async Task<IReadOnlyList<ViajeModel>> GetbyFecha(DateTime fecha)
            => await QueryAsync("sp_Viaje_GetByFecha", ViajeMapper.Map, Param("@Fecha", fecha.Date));

        public async Task<IReadOnlyList<ViajeModel>> GetbyConductor(int conductorId)
            => await QueryAsync("sp_Viaje_GetByConductor", ViajeMapper.Map, Param("@ConductorId", conductorId));

        public async Task<IReadOnlyList<ViajeModel>> GetbyAutobusActivo(int autobusId)
            => await QueryAsync("sp_Viaje_GetByAutobusActivo", ViajeMapper.Map, Param("@AutobusId", autobusId));

        public async Task<IReadOnlyList<ViajeModel>> GetbyPeriodo(DateTime desde, DateTime hasta)
            => await QueryAsync("sp_Viaje_GetByPeriodo", ViajeMapper.Map,
                Param("@Desde", desde), Param("@Hasta", hasta));

        public async Task<IReadOnlyList<IncidenciaModel>> GetIncidenciasbyPeriodo(DateTime desde, DateTime hasta)
            => await QueryAsync("sp_Incidencia_GetByPeriodo", IncidenciaMapper.Map,
                Param("@Desde", desde), Param("@Hasta", hasta));

        public async Task AddAsync(Viaje entity)
            => entity.Id = await ExecuteScalarAsync("sp_Viaje_Insert", ViajeParameters.ParaInsertar(entity));

        public async Task UpdateAsync(Viaje entity)
            => await ExecuteAsync("sp_Viaje_Update", ViajeParameters.ParaActualizar(entity));

        public async Task DeleteAsync(Viaje entity)
            => await ExecuteAsync("sp_Viaje_Delete", ViajeParameters.ParaEliminar(entity));

        public async Task AddIncidencia(Incidencia incidencia)
            => incidencia.Id = await ExecuteScalarAsync("sp_Incidencia_Insert", IncidenciaParameters.ParaInsertar(incidencia));

        public async Task<int> CancelarViajeAsync(
            int viajeId, int conductorId, string motivo, DateTime fechaHora, int canceladoPorId, string creadoPor)
            => await ExecuteScalarAsync("sp_CancelarViaje",
                Param("@ViajeId", viajeId),
                Param("@ConductorId", conductorId),
                Param("@Motivo", motivo),
                Param("@FechaHora", fechaHora),
                Param("@CanceladoPorUsuarioId", canceladoPorId),
                Param("@CreadoPor", creadoPor));
    }

    internal static class ViajeMapper
    {
        internal static ViajeModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            RutaId = r.Int("RutaId"),
            HorarioRutaId = r.Int("HorarioRutaId"),
            AutobusId = r.Int("AutobusId"),
            ConductorId = r.Int("ConductorId"),
            Fecha = r.DateTime("Fecha"),
            Estado = r.Enum<EstadoViaje>("Estado"),
            HoraInicioReal = r.NullableDateTime("HoraInicioReal"),
            HoraFinReal = r.NullableDateTime("HoraFinReal"),
            CupoActual = r.Int("CupoActual"),
            CapacidadMaxima = r.Int("CapacidadMaxima")
        };
    }

    internal static class IncidenciaMapper
    {
        internal static IncidenciaModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            ViajeId = r.Int("ViajeId"),
            ConductorId = r.Int("ConductorId"),
            Tipo = r.Str("Tipo"),
            Descripcion = r.Str("Descripcion"),
            FechaHora = r.DateTime("FechaHora")
        };
    }

    internal static class ViajeParameters
    {
        internal static SqlParameter[] ParaInsertar(Viaje v) =>
        [
            SqlRepositoryBase.Param("@RutaId", v.RutaId),
            SqlRepositoryBase.Param("@HorarioRutaId", v.HorarioRutaId),
            SqlRepositoryBase.Param("@AutobusId",  v.AutobusId),
            SqlRepositoryBase.Param("@ConductorId", v.ConductorId),
            SqlRepositoryBase.Param("@Fecha", v.Fecha),
            SqlRepositoryBase.Param("@Estado", (int)v.Estado),
            SqlRepositoryBase.Param("@HoraInicioReal", v.HoraInicioReal),
            SqlRepositoryBase.Param("@HoraFinReal", v.HoraFinReal),
            SqlRepositoryBase.Param("@CupoActual", v.CupoActual),
            SqlRepositoryBase.Param("@CapacidadMaxima",v.CapacidadMaxima),
            SqlRepositoryBase.Param("@CreadoPor", v.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(Viaje v) =>
        [
            SqlRepositoryBase.Param("@Id", v.Id),
            ..ParaInsertar(v)
        ];

        internal static SqlParameter[] ParaEliminar(Viaje v) =>
        [
            SqlRepositoryBase.Param("@Id", v.Id),
            SqlRepositoryBase.Param("@EliminadoPor", v.EliminadoPor)
        ];
    }

    internal static class IncidenciaParameters
    {
        internal static SqlParameter[] ParaInsertar(Incidencia i) =>
        [
            SqlRepositoryBase.Param("@ViajeId", i.ViajeId),
            SqlRepositoryBase.Param("@ConductorId", i.ConductorId),
            SqlRepositoryBase.Param("@Tipo", i.Tipo),
            SqlRepositoryBase.Param("@Descripcion", i.Descripcion),
            SqlRepositoryBase.Param("@FechaHora", i.FechaHora),
            SqlRepositoryBase.Param("@CreadoPor", i.CreadoPor)
        ];
    }
}