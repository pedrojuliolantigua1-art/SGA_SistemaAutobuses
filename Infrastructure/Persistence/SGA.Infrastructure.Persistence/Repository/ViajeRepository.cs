using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{

    public sealed class ViajeRepository : SqlRepositoryBase, IViajeRepository
    {
        public ViajeRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<Viaje?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_Viaje_GetById", ViajeMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<Viaje>> GetAllAsync()
            => await QueryAsync("sp_Viaje_GetAll", ViajeMapper.Map);

        public async Task AddAsync(Viaje entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_Viaje_Insert",
                ViajeParameters.ParaInsertar(entity));

        public async Task UpdateAsync(Viaje entity)
            => await ExecuteAsync("sp_Viaje_Update",
                ViajeParameters.ParaActualizar(entity));

        public async Task DeleteAsync(Viaje entity)
            => await ExecuteAsync("sp_Viaje_Delete",
                ViajeParameters.ParaEliminar(entity));

        public async Task<IReadOnlyList<Viaje>> Getby_Fecha(DateTime fecha)
            => await QueryAsync(
                "sp_Viaje_GetByFecha", ViajeMapper.Map,
                Param("@Fecha", fecha.Date));

        public async Task<IReadOnlyList<Viaje>> Getby_Conductor(int conductorId)
            => await QueryAsync(
                "sp_Viaje_GetByConductor", ViajeMapper.Map,
                Param("@ConductorId", conductorId));

        public async Task<IReadOnlyList<Viaje>> Getby_AutobusActivo(int autobusId)
            => await QueryAsync(
                "sp_Viaje_GetByAutobusActivo", ViajeMapper.Map,
                Param("@AutobusId", autobusId));

        public async Task<IReadOnlyList<Viaje>> Getby_Periodo(DateTime desde, DateTime hasta)
            => await QueryAsync(
                "sp_Viaje_GetByPeriodo", ViajeMapper.Map,
                Param("@Desde", desde),
                Param("@Hasta", hasta));

        public async Task AddIncidencia(Incidencia incidencia)
            => incidencia.Id = await ExecuteScalarAsync(
                "sp_Incidencia_Insert",
                IncidenciaParameters.ParaInsertar(incidencia));

        public async Task<IReadOnlyList<Incidencia>> GetIncidencias_byPeriodo(
            DateTime desde, DateTime hasta)
            => await QueryAsync(
                "sp_Incidencia_GetByPeriodo", IncidenciaMapper.Map,
                Param("@Desde", desde),
                Param("@Hasta", hasta));
    }

    internal static class ViajeMapper
    {
        internal static Viaje Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            RutaId = SqlRepositoryBase.GetInt(r, "RutaId"),
            HorarioRutaId = SqlRepositoryBase.GetInt(r, "HorarioRutaId"),
            AutobusId = SqlRepositoryBase.GetInt(r, "AutobusId"),
            ConductorId = SqlRepositoryBase.GetInt(r, "ConductorId"),
            Fecha = SqlRepositoryBase.GetDateTime(r, "Fecha"),
            Estado = SqlRepositoryBase.GetEnum<EstadoViaje>(r, "Estado"),
            HoraInicioReal = SqlRepositoryBase.GetNullableDateTime(r, "HoraInicioReal"),
            HoraFinReal = SqlRepositoryBase.GetNullableDateTime(r, "HoraFinReal"),
            CupoActual = SqlRepositoryBase.GetInt(r, "CupoActual"),
            CapacidadMaxima = SqlRepositoryBase.GetInt(r, "CapacidadMaxima"),
            FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion"),
            CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor"),
            FechaModificacion = SqlRepositoryBase.GetDateTime(r, "FechaModificacion"),
            Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado"),
            EliminadoPor = SqlRepositoryBase.GetString(r, "EliminadoPor")
        };
    }

    internal static class IncidenciaMapper
    {
        internal static Incidencia Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            ViajeId = SqlRepositoryBase.GetInt(r, "ViajeId"),
            ConductorId = SqlRepositoryBase.GetInt(r, "ConductorId"),
            Tipo = SqlRepositoryBase.GetString(r, "Tipo"),
            Descripcion = SqlRepositoryBase.GetString(r, "Descripcion"),
            FechaHora = SqlRepositoryBase.GetDateTime(r, "FechaHora")
        };
    }

    internal static class ViajeParameters
    {
        internal static SqlParameter[] ParaInsertar(Viaje v) =>
        [
            SqlRepositoryBase.Param("@RutaId",         v.RutaId),
            SqlRepositoryBase.Param("@HorarioRutaId",  v.HorarioRutaId),
            SqlRepositoryBase.Param("@AutobusId",       v.AutobusId),
            SqlRepositoryBase.Param("@ConductorId",     v.ConductorId),
            SqlRepositoryBase.Param("@Fecha",           v.Fecha),
            SqlRepositoryBase.Param("@Estado",          (int)v.Estado),
            SqlRepositoryBase.Param("@HoraInicioReal",  v.HoraInicioReal),
            SqlRepositoryBase.Param("@HoraFinReal",     v.HoraFinReal),
            SqlRepositoryBase.Param("@CupoActual",      v.CupoActual),
            SqlRepositoryBase.Param("@CapacidadMaxima", v.CapacidadMaxima),
            SqlRepositoryBase.Param("@CreadoPor",       v.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(Viaje v) =>
        [
            SqlRepositoryBase.Param("@Id", v.Id),
            ..ParaInsertar(v)
        ];

        internal static SqlParameter[] ParaEliminar(Viaje v) =>
        [
            SqlRepositoryBase.Param("@Id",           v.Id),
            SqlRepositoryBase.Param("@EliminadoPor", v.EliminadoPor)
        ];
    }

    internal static class IncidenciaParameters
    {
        internal static SqlParameter[] ParaInsertar(Incidencia i) =>
        [
            SqlRepositoryBase.Param("@ViajeId",     i.ViajeId),
            SqlRepositoryBase.Param("@ConductorId", i.ConductorId),
            SqlRepositoryBase.Param("@Tipo",        i.Tipo),
            SqlRepositoryBase.Param("@Descripcion", i.Descripcion),
            SqlRepositoryBase.Param("@FechaHora",   i.FechaHora),
            SqlRepositoryBase.Param("@CreadoPor",   i.CreadoPor)
        ];
    }
}
