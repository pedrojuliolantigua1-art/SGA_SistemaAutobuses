using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Fotos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Conexion;

namespace SGA.Infrastructure.Persistence.Repositories
{

    public sealed class FotoIncidenciaRepository : SqlRepositoryBase, IFotoIncidenciaRepository
    {
        public FotoIncidenciaRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<FotoIncidencia?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_FotoIncidencia_GetById", FotoIncidenciaMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<FotoIncidencia>> GetAllAsync()
            => await QueryAsync("sp_FotoIncidencia_GetAll", FotoIncidenciaMapper.Map);

        public async Task AddAsync(FotoIncidencia entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_FotoIncidencia_Insert",
                FotoIncidenciaParameters.ParaInsertar(entity));

        public async Task UpdateAsync(FotoIncidencia entity)
            => await ExecuteAsync("sp_FotoIncidencia_Update",
                FotoIncidenciaParameters.ParaActualizar(entity));

        public async Task DeleteAsync(FotoIncidencia entity)
            => await ExecuteAsync("sp_FotoIncidencia_Delete",
                Param("@Id", entity.Id),
                Param("@EliminadoPor", entity.EliminadoPor));

        public async Task<IReadOnlyList<FotoIncidencia>> GetByIncidenciaId(int incidenciaId)
            => await QueryAsync(
                "sp_FotoIncidencia_GetByIncidenciaId", FotoIncidenciaMapper.Map,
                Param("@IncidenciaId", incidenciaId));

        public async Task<IReadOnlyList<FotoIncidencia>> GetByViajeId(int viajeId)
            => await QueryAsync(
                "sp_FotoIncidencia_GetByViajeId", FotoIncidenciaMapper.Map,
                Param("@ViajeId", viajeId));
    }

    internal static class FotoIncidenciaMapper
    {
        internal static FotoIncidencia Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            IncidenciaId = SqlRepositoryBase.GetInt(r, "IncidenciaId"),
            NombreArchivo = SqlRepositoryBase.GetString(r, "NombreArchivo") ?? string.Empty,
            UrlPublica = SqlRepositoryBase.GetString(r, "UrlPublica") ?? string.Empty,
            PublicId = SqlRepositoryBase.GetString(r, "PublicId") ?? string.Empty,
            SubidoPor = SqlRepositoryBase.GetString(r, "SubidoPor") ?? string.Empty,
            FechaSubida = SqlRepositoryBase.GetDateTime(r, "FechaSubida"),
            FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion"),
            CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor"),
            FechaModificacion = SqlRepositoryBase.GetDateTime(r, "FechaModificacion"),
            Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado"),
            EliminadoPor = SqlRepositoryBase.GetString(r, "EliminadoPor")
        };
    }

    internal static class FotoIncidenciaParameters
    {
        internal static SqlParameter[] ParaInsertar(FotoIncidencia f) =>
        [
            SqlRepositoryBase.Param("@IncidenciaId",  f.IncidenciaId),
            SqlRepositoryBase.Param("@NombreArchivo", f.NombreArchivo),
            SqlRepositoryBase.Param("@UrlPublica",    f.UrlPublica),
            SqlRepositoryBase.Param("@PublicId",      f.PublicId),
            SqlRepositoryBase.Param("@SubidoPor",     f.SubidoPor),
            SqlRepositoryBase.Param("@FechaSubida",   f.FechaSubida),
            SqlRepositoryBase.Param("@CreadoPor",     f.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(FotoIncidencia f) =>
        [
            SqlRepositoryBase.Param("@Id", f.Id),
            ..ParaInsertar(f)
        ];
    }
}