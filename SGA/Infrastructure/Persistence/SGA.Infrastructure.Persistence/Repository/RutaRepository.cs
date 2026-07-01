using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class RutaRepository : SqlRepositoryBase, IRutaRepository
    {
        public RutaRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<RutaModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_Ruta_GetById", RutaMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<RutaModel>> GetAllAsync()
            => await QueryAsync("sp_Ruta_GetAll", RutaMapper.Map);

        public async Task<IReadOnlyList<RutaModel>> GetActivas()
            => await QueryAsync("sp_Ruta_GetActivas", RutaMapper.Map);

        public async Task<IReadOnlyList<ParadaModel>> GetParadas(int rutaId)
            => await QueryAsync("sp_Parada_GetByRuta", ParadaMapper.Map, Param("@RutaId", rutaId));

        public async Task<IReadOnlyList<HorarioModel>> GetHorarios(int rutaId)
            => await QueryAsync("sp_HorarioRuta_GetByRuta", HorarioMapper.Map, Param("@RutaId", rutaId));

        public async Task AddAsync(Ruta entity)
            => entity.Id = await ExecuteScalarAsync("sp_Ruta_Insert", RutaParameters.ParaInsertar(entity));

        public async Task UpdateAsync(Ruta entity)
            => await ExecuteAsync("sp_Ruta_Update", RutaParameters.ParaActualizar(entity));

        public async Task DeleteAsync(Ruta entity)
            => await ExecuteAsync("sp_Ruta_Delete", RutaParameters.ParaEliminar(entity));
    }

    internal static class RutaMapper
    {
        internal static RutaModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            Nombre = r.Str("Nombre"),
            Descripcion = r.Str("Descripcion"),
            Activa = r.Bool("Activa")
        };
    }

    internal static class RutaParameters
    {
        internal static SqlParameter[] ParaInsertar(Ruta r) =>
        [
            SqlRepositoryBase.Param("@Nombre", r.Nombre),
            SqlRepositoryBase.Param("@Descripcion", r.Descripcion),
            SqlRepositoryBase.Param("@Activa", r.Activa),
            SqlRepositoryBase.Param("@CreadoPor",r.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(Ruta r) =>
        [
            SqlRepositoryBase.Param("@Id", r.Id),
            ..ParaInsertar(r)
        ];

        internal static SqlParameter[] ParaEliminar(Ruta r) =>
        [
            SqlRepositoryBase.Param("@Id", r.Id),
            SqlRepositoryBase.Param("@EliminadoPor", r.EliminadoPor)
        ];
    }
}