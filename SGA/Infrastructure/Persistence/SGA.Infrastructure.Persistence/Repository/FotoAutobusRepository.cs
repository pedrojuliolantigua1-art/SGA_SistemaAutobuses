using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Fotos;
using SGA.Domain.Models.Fotos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class FotoAutobusRepository : SqlRepositoryBase, IFotoAutobusRepository
    {
        public FotoAutobusRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<FotoAutobusModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_FotoAutobus_GetById", FotoAutobusMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<FotoAutobusModel>> GetAllAsync()
            => await QueryAsync("sp_FotoAutobus_GetAll", FotoAutobusMapper.Map);

        public async Task<FotoAutobusModel?> GetByAutobusId(int autobusId)
            => await QuerySingleOrDefaultAsync("sp_FotoAutobus_GetByAutobusId", FotoAutobusMapper.Map, Param("@AutobusId", autobusId));

        public async Task<IReadOnlyList<FotoAutobusModel>> GetAllByAutobusId(int autobusId)
            => await QueryAsync("sp_FotoAutobus_GetAllByAutobusId", FotoAutobusMapper.Map, Param("@AutobusId", autobusId));

        public async Task AddAsync(FotoAutobus entity)
            => entity.Id = await ExecuteScalarAsync("sp_FotoAutobus_Insert", FotoAutobusParameters.ParaInsertar(entity));

        public async Task UpdateAsync(FotoAutobus entity)
            => await ExecuteAsync("sp_FotoAutobus_Update", FotoAutobusParameters.ParaActualizar(entity));

        public async Task DeleteAsync(FotoAutobus entity)
            => await ExecuteAsync("sp_FotoAutobus_Delete",
                Param("@Id", entity.Id),
                Param("@EliminadoPor", entity.EliminadoPor));
    }

    internal static class FotoAutobusMapper
    {
        internal static FotoAutobusModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            AutobusId = r.Int("AutobusId"),
            NombreArchivo = r.Str("NombreArchivo") ?? string.Empty,
            UrlPublica = r.Str("UrlPublica") ?? string.Empty,
            PublicId = r.Str("PublicId") ?? string.Empty,
            SubidoPor = r.Str("SubidoPor") ?? string.Empty,
            FechaSubida = r.DateTime("FechaSubida")
        };
    }

    internal static class FotoAutobusParameters
    {
        internal static SqlParameter[] ParaInsertar(FotoAutobus f) =>
        [
            SqlRepositoryBase.Param("@AutobusId",f.AutobusId),
            SqlRepositoryBase.Param("@NombreArchivo",f.NombreArchivo),
            SqlRepositoryBase.Param("@UrlPublica",f.UrlPublica),
            SqlRepositoryBase.Param("@PublicId", f.PublicId),
            SqlRepositoryBase.Param("@SubidoPor", f.SubidoPor),
            SqlRepositoryBase.Param("@FechaSubida", f.FechaSubida),
            SqlRepositoryBase.Param("@CreadoPor", f.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(FotoAutobus f) =>
        [
            SqlRepositoryBase.Param("@Id", f.Id),
            ..ParaInsertar(f)
        ];
    }
}