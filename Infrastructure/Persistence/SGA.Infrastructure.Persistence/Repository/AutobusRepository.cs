using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Conexion;

namespace SGA.Infrastructure.Persistence.Repositories
{

    public sealed class AutobusRepository : SqlRepositoryBase, IAutobusRepository
    {
        public AutobusRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<Autobus?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_Autobus_GetById", AutobusMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<Autobus>> GetAllAsync()
            => await QueryAsync("sp_Autobus_GetAll", AutobusMapper.Map);

        public async Task AddAsync(Autobus entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_Autobus_Insert",
                AutobusParameters.ParaInsertar(entity));

        public async Task UpdateAsync(Autobus entity)
            => await ExecuteAsync("sp_Autobus_Update",
                AutobusParameters.ParaActualizar(entity));

        public async Task DeleteAsync(Autobus entity)
            => await ExecuteAsync("sp_Autobus_Delete",
                AutobusParameters.ParaEliminar(entity));

        public async Task<IReadOnlyList<Autobus>> GetDisponibles()
            => await QueryAsync("sp_Autobus_GetDisponibles", AutobusMapper.Map);
    }

    internal static class AutobusMapper
    {
        internal static Autobus Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            Placa = SqlRepositoryBase.GetString(r, "Placa"),
            Modelo = SqlRepositoryBase.GetString(r, "Modelo"),
            Capacidad = SqlRepositoryBase.GetInt(r, "Capacidad"),
            Estado = SqlRepositoryBase.GetString(r, "Estado") ?? "Disponible",
            FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion"),
            CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor"),
            FechaModificacion = SqlRepositoryBase.GetDateTime(r, "FechaModificacion"),
            Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado"),
            EliminadoPor = SqlRepositoryBase.GetString(r, "EliminadoPor")
        };
    }

    internal static class AutobusParameters
    {
        internal static SqlParameter[] ParaInsertar(Autobus a) =>
        [
            SqlRepositoryBase.Param("@Placa",     a.Placa),
            SqlRepositoryBase.Param("@Modelo",    a.Modelo),
            SqlRepositoryBase.Param("@Capacidad", a.Capacidad),
            SqlRepositoryBase.Param("@Estado",    a.Estado),
            SqlRepositoryBase.Param("@CreadoPor", a.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(Autobus a) =>
        [
            SqlRepositoryBase.Param("@Id", a.Id),
            ..ParaInsertar(a)
        ];

        internal static SqlParameter[] ParaEliminar(Autobus a) =>
        [
            SqlRepositoryBase.Param("@Id",           a.Id),
            SqlRepositoryBase.Param("@EliminadoPor", a.EliminadoPor)
        ];
    }
}
