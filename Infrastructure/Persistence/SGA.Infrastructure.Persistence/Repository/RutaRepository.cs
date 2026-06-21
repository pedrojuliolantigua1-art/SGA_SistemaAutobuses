using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    
    public sealed class RutaRepository : SqlRepositoryBase, IRutaRepository
    {
        public RutaRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<Ruta?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_Ruta_GetById", RutaMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<Ruta>> GetAllAsync()
            => await QueryAsync("sp_Ruta_GetAll", RutaMapper.Map);

        public async Task AddAsync(Ruta entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_Ruta_Insert",
                RutaParameters.ParaInsertar(entity));

        public async Task UpdateAsync(Ruta entity)
            => await ExecuteAsync("sp_Ruta_Update",
                RutaParameters.ParaActualizar(entity));

        public async Task DeleteAsync(Ruta entity)
            => await ExecuteAsync("sp_Ruta_Delete",
                RutaParameters.ParaEliminar(entity));

        public async Task<IReadOnlyList<Ruta>> GetActivas()
            => await QueryAsync("sp_Ruta_GetActivas", RutaMapper.Map);

        // Usa el mapper de ParadaRepository — no duplica logica
        public async Task<IReadOnlyList<Parada>> GetParadas(int rutaId)
            => await QueryAsync(
                "sp_Parada_GetByRuta", ParadaMapper.Map,
                Param("@RutaId", rutaId));

        // Usa el mapper de HorarioRutaRepository — no duplica logica
        public async Task<IReadOnlyList<HorarioRuta>> GetHorarios(int rutaId)
            => await QueryAsync(
                "sp_HorarioRuta_GetByRuta", HorarioRutaMapper.Map,
                Param("@RutaId", rutaId));
    }

    internal static class RutaMapper
    {
        internal static Ruta Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            Nombre = SqlRepositoryBase.GetString(r, "Nombre"),
            Descripcion = SqlRepositoryBase.GetString(r, "Descripcion"),
            Activa = SqlRepositoryBase.GetBool(r, "Activa"),
            FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion"),
            CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor"),
            FechaModificacion = SqlRepositoryBase.GetDateTime(r, "FechaModificacion"),
            Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado"),
            EliminadoPor = SqlRepositoryBase.GetString(r, "EliminadoPor")
        };
    }

    internal static class RutaParameters
    {
        internal static SqlParameter[] ParaInsertar(Ruta r) =>
        [
            SqlRepositoryBase.Param("@Nombre",      r.Nombre),
            SqlRepositoryBase.Param("@Descripcion", r.Descripcion),
            SqlRepositoryBase.Param("@Activa",      r.Activa),
            SqlRepositoryBase.Param("@CreadoPor",   r.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(Ruta r) =>
        [
            SqlRepositoryBase.Param("@Id", r.Id),
            ..ParaInsertar(r)
        ];

        internal static SqlParameter[] ParaEliminar(Ruta r) =>
        [
            SqlRepositoryBase.Param("@Id",           r.Id),
            SqlRepositoryBase.Param("@EliminadoPor", r.EliminadoPor)
        ];
    }
}
