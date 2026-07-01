using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Fotos;
using SGA.Domain.Models.Fotos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class FotoIncidenciaRepository : SqlRepositoryBase, IFotoIncidenciaRepository
    {
        public FotoIncidenciaRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<FotoIncidenciaModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_FotoIncidencia_GetById", FotoIncidenciaMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<FotoIncidenciaModel>> GetAllAsync()
            => await QueryAsync("sp_FotoIncidencia_GetAll", FotoIncidenciaMapper.Map);

        public async Task<IReadOnlyList<FotoIncidenciaModel>> GetByIncidenciaId(int incidenciaId)
            => await QueryAsync("sp_FotoIncidencia_GetByIncidenciaId", FotoIncidenciaMapper.Map, Param("@IncidenciaId", incidenciaId));

        public async Task<IReadOnlyList<FotoIncidenciaModel>> GetByViajeId(int viajeId)
            => await QueryAsync("sp_FotoIncidencia_GetByViajeId", FotoIncidenciaMapper.Map, Param("@ViajeId", viajeId));

        public async Task AddAsync(FotoIncidencia entity)
            => entity.Id = await ExecuteScalarAsync("sp_FotoIncidencia_Insert", FotoIncidenciaParameters.ParaInsertar(entity));

        public async Task UpdateAsync(FotoIncidencia entity)
            => await ExecuteAsync("sp_FotoIncidencia_Update", FotoIncidenciaParameters.ParaActualizar(entity));

        public async Task DeleteAsync(FotoIncidencia entity)
            => await ExecuteAsync("sp_FotoIncidencia_Delete",
                Param("@Id", entity.Id),
                Param("@EliminadoPor", entity.EliminadoPor));
    }

    internal static class FotoIncidenciaMapper
    {
        internal static FotoIncidenciaModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            IncidenciaId = r.Int("IncidenciaId"),
            NombreArchivo = r.Str("NombreArchivo") ?? string.Empty,
            UrlPublica = r.Str("UrlPublica") ?? string.Empty,
            PublicId = r.Str("PublicId") ?? string.Empty,
            SubidoPor = r.Str("SubidoPor") ?? string.Empty,
            FechaSubida = r.DateTime("FechaSubida")
        };
    }

    internal static class FotoIncidenciaParameters
    {
        internal static SqlParameter[] ParaInsertar(FotoIncidencia f) =>
        [
            SqlRepositoryBase.Param("@IncidenciaId", f.IncidenciaId),
            SqlRepositoryBase.Param("@NombreArchivo", f.NombreArchivo),
            SqlRepositoryBase.Param("@UrlPublica", f.UrlPublica),
            SqlRepositoryBase.Param("@PublicId",f.PublicId),
            SqlRepositoryBase.Param("@SubidoPor", f.SubidoPor),
            SqlRepositoryBase.Param("@FechaSubida", f.FechaSubida),
            SqlRepositoryBase.Param("@CreadoPor", f.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(FotoIncidencia f) =>
        [
            SqlRepositoryBase.Param("@Id", f.Id),
            ..ParaInsertar(f)
        ];
    }
}