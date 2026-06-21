using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Conexion;

namespace SGA.Infrastructure.Persistence.Repositories
{

    public sealed class HorarioRutaRepository : SqlRepositoryBase, IHorarioRutaRepository
    {
        public HorarioRutaRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<HorarioRuta?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_HorarioRuta_GetById", HorarioRutaMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<HorarioRuta>> GetAllAsync()
            => await QueryAsync("sp_HorarioRuta_GetAll", HorarioRutaMapper.Map);

        public async Task AddAsync(HorarioRuta entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_HorarioRuta_Insert",
                HorarioRutaParameters.ParaInsertar(entity));

        public async Task UpdateAsync(HorarioRuta entity)
            => await ExecuteAsync("sp_HorarioRuta_Update",
                HorarioRutaParameters.ParaActualizar(entity));

        public async Task DeleteAsync(HorarioRuta entity)
            => await ExecuteAsync("sp_HorarioRuta_Delete",
                HorarioRutaParameters.ParaEliminar(entity));

        public async Task<IReadOnlyList<HorarioRuta>> GetByRuta(int rutaId)
            => await QueryAsync(
                "sp_HorarioRuta_GetByRuta", HorarioRutaMapper.Map,
                Param("@RutaId", rutaId));
    }

    internal static class HorarioRutaMapper
    {
        internal static HorarioRuta Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            RutaId = SqlRepositoryBase.GetInt(r, "RutaId"),
            HoraSalida = SqlRepositoryBase.GetTimeSpan(r, "HoraSalida"),
            HoraLlegadaEstimada = SqlRepositoryBase.GetTimeSpan(r, "HoraLlegadaEstimada"),
            Activo = SqlRepositoryBase.GetBool(r, "Activo"),
            FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion"),
            CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor"),
            FechaModificacion = SqlRepositoryBase.GetDateTime(r, "FechaModificacion"),
            Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado"),
            EliminadoPor = SqlRepositoryBase.GetString(r, "EliminadoPor")
        };
    }

    internal static class HorarioRutaParameters
    {
        internal static SqlParameter[] ParaInsertar(HorarioRuta h) =>
        [
            SqlRepositoryBase.Param("@RutaId",              h.RutaId),
            SqlRepositoryBase.Param("@HoraSalida",          h.HoraSalida),
            SqlRepositoryBase.Param("@HoraLlegadaEstimada", h.HoraLlegadaEstimada),
            SqlRepositoryBase.Param("@Activo",              h.Activo),
            SqlRepositoryBase.Param("@CreadoPor",           h.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(HorarioRuta h) =>
        [
            SqlRepositoryBase.Param("@Id", h.Id),
            ..ParaInsertar(h)
        ];

        internal static SqlParameter[] ParaEliminar(HorarioRuta h) =>
        [
            SqlRepositoryBase.Param("@Id",           h.Id),
            SqlRepositoryBase.Param("@EliminadoPor", h.EliminadoPor)
        ];
    }
}
