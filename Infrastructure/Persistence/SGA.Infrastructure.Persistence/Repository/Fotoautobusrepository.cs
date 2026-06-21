using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Fotos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Conexion;

namespace SGA.Infrastructure.Persistence.Repositories
{

    public sealed class FotoAutobusRepository : SqlRepositoryBase, IFotoAutobusRepository
    {
        public FotoAutobusRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<FotoAutobus?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_FotoAutobus_GetById", FotoAutobusMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<FotoAutobus>> GetAllAsync()
            => await QueryAsync("sp_FotoAutobus_GetAll", FotoAutobusMapper.Map);

        public async Task AddAsync(FotoAutobus entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_FotoAutobus_Insert",
                FotoAutobusParameters.ParaInsertar(entity));

        public async Task UpdateAsync(FotoAutobus entity)
            => await ExecuteAsync("sp_FotoAutobus_Update",
                FotoAutobusParameters.ParaActualizar(entity));

        public async Task DeleteAsync(FotoAutobus entity)
            => await ExecuteAsync("sp_FotoAutobus_Delete",
                Param("@Id", entity.Id),
                Param("@EliminadoPor", entity.EliminadoPor));

        public async Task<FotoAutobus?> GetByAutobusId(int autobusId)
            => await QuerySingleOrDefaultAsync(
                "sp_FotoAutobus_GetByAutobusId", FotoAutobusMapper.Map,
                Param("@AutobusId", autobusId));

        public async Task<IReadOnlyList<FotoAutobus>> GetAllByAutobusId(int autobusId)
            => await QueryAsync(
                "sp_FotoAutobus_GetAllByAutobusId", FotoAutobusMapper.Map,
                Param("@AutobusId", autobusId));
    }

    internal static class FotoAutobusMapper
    {
        internal static FotoAutobus Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            AutobusId = SqlRepositoryBase.GetInt(r, "AutobusId"),
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

    internal static class FotoAutobusParameters
    {
        internal static SqlParameter[] ParaInsertar(FotoAutobus f) =>
        [
            SqlRepositoryBase.Param("@AutobusId",     f.AutobusId),
            SqlRepositoryBase.Param("@NombreArchivo", f.NombreArchivo),
            SqlRepositoryBase.Param("@UrlPublica",    f.UrlPublica),
            SqlRepositoryBase.Param("@PublicId",      f.PublicId),
            SqlRepositoryBase.Param("@SubidoPor",     f.SubidoPor),
            SqlRepositoryBase.Param("@FechaSubida",   f.FechaSubida),
            SqlRepositoryBase.Param("@CreadoPor",     f.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(FotoAutobus f) =>
        [
            SqlRepositoryBase.Param("@Id", f.Id),
            ..ParaInsertar(f)
        ];
    }
}