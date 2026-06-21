using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Conexion;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class ParadaRepository : SqlRepositoryBase, IParadaRepository
    {
        public ParadaRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<Parada?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_Parada_GetById", ParadaMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<Parada>> GetAllAsync()
            => await QueryAsync("sp_Parada_GetAll", ParadaMapper.Map);

        public async Task AddAsync(Parada entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_Parada_Insert",
                ParadaParameters.ParaInsertar(entity));

        public async Task UpdateAsync(Parada entity)
            => await ExecuteAsync("sp_Parada_Update",
                ParadaParameters.ParaActualizar(entity));

        public async Task DeleteAsync(Parada entity)
            => await ExecuteAsync("sp_Parada_Delete",
                ParadaParameters.ParaEliminar(entity));

        public async Task<IReadOnlyList<Parada>> GetByRuta(int rutaId)
            => await QueryAsync(
                "sp_Parada_GetByRuta", ParadaMapper.Map,
                Param("@RutaId", rutaId));
    }

    internal static class ParadaMapper
    {
        internal static Parada Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            RutaId = SqlRepositoryBase.GetInt(r, "RutaId"),
            Nombre = SqlRepositoryBase.GetString(r, "Nombre"),
            Referencia = SqlRepositoryBase.GetString(r, "Referencia"),
            Orden = SqlRepositoryBase.GetInt(r, "Orden"),
            FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion"),
            CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor"),
            FechaModificacion = SqlRepositoryBase.GetDateTime(r, "FechaModificacion"),
            Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado"),
            EliminadoPor = SqlRepositoryBase.GetString(r, "EliminadoPor")
        };
    }

    internal static class ParadaParameters
    {
        internal static SqlParameter[] ParaInsertar(Parada p) =>
        [
            SqlRepositoryBase.Param("@RutaId",     p.RutaId),
            SqlRepositoryBase.Param("@Nombre",     p.Nombre),
            SqlRepositoryBase.Param("@Referencia", p.Referencia),
            SqlRepositoryBase.Param("@Orden",      p.Orden),
            SqlRepositoryBase.Param("@CreadoPor",  p.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(Parada p) =>
        [
            SqlRepositoryBase.Param("@Id", p.Id),
            ..ParaInsertar(p)
        ];

        internal static SqlParameter[] ParaEliminar(Parada p) =>
        [
            SqlRepositoryBase.Param("@Id",           p.Id),
            SqlRepositoryBase.Param("@EliminadoPor", p.EliminadoPor)
        ];
    }
}
