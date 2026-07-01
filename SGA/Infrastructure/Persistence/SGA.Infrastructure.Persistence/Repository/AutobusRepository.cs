using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class AutobusRepository : SqlRepositoryBase, IAutobusRepository
    {
        public AutobusRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<AutobusModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_Autobus_GetById", AutobusMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<AutobusModel>> GetAllAsync()
            => await QueryAsync("sp_Autobus_GetAll", AutobusMapper.Map);

        public async Task<IReadOnlyList<AutobusModel>> GetDisponibles()
            => await QueryAsync("sp_Autobus_GetDisponibles", AutobusMapper.Map);

        public async Task AddAsync(Autobus entity)
            => entity.Id = await ExecuteScalarAsync("sp_Autobus_Insert", AutobusParameters.ParaInsertar(entity));

        public async Task UpdateAsync(Autobus entity)
            => await ExecuteAsync("sp_Autobus_Update", AutobusParameters.ParaActualizar(entity));

        public async Task DeleteAsync(Autobus entity)
            => await ExecuteAsync("sp_Autobus_Delete", AutobusParameters.ParaEliminar(entity));
    }

    internal static class AutobusMapper
    {
        internal static AutobusModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            Placa = r.Str("Placa"),
            Modelo = r.Str("Modelo"),
            Capacidad = r.Int("Capacidad"),
            Estado = r.Str("Estado") ?? "Disponible"
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