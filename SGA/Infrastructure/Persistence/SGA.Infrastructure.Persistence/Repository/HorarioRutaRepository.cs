using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class HorarioRutaRepository : SqlRepositoryBase, IHorarioRutaRepository
    {
        public HorarioRutaRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<HorarioModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_HorarioRuta_GetById", HorarioMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<HorarioModel>> GetAllAsync()
            => await QueryAsync("sp_HorarioRuta_GetAll", HorarioMapper.Map);

        public async Task<IReadOnlyList<HorarioModel>> GetByRuta(int rutaId)
            => await QueryAsync("sp_HorarioRuta_GetByRuta", HorarioMapper.Map, Param("@RutaId", rutaId));

        public async Task AddAsync(HorarioRuta entity)
            => entity.Id = await ExecuteScalarAsync("sp_HorarioRuta_Insert", HorarioParameters.ParaInsertar(entity));

        public async Task UpdateAsync(HorarioRuta entity)
            => await ExecuteAsync("sp_HorarioRuta_Update", HorarioParameters.ParaActualizar(entity));

        public async Task DeleteAsync(HorarioRuta entity)
            => await ExecuteAsync("sp_HorarioRuta_Delete", HorarioParameters.ParaEliminar(entity));
    }

    internal static class HorarioMapper
    {
        internal static HorarioModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            RutaId = r.Int("RutaId"),
            HoraSalida = r.TimeSpan("HoraSalida"),
            HoraLlegadaEstimada = r.TimeSpan("HoraLlegadaEstimada"),
            Activo = r.Bool("Activo")
        };
    }

    internal static class HorarioParameters
    {
        internal static SqlParameter[] ParaInsertar(HorarioRuta h) =>
        [
            SqlRepositoryBase.Param("@RutaId", h.RutaId),
            SqlRepositoryBase.Param("@HoraSalida", h.HoraSalida),
            SqlRepositoryBase.Param("@HoraLlegadaEstimada", h.HoraLlegadaEstimada),
            SqlRepositoryBase.Param("@Activo", h.Activo),
            SqlRepositoryBase.Param("@CreadoPor", h.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(HorarioRuta h) =>
        [
            SqlRepositoryBase.Param("@Id", h.Id),
            ..ParaInsertar(h)
        ];

        internal static SqlParameter[] ParaEliminar(HorarioRuta h) =>
        [
            SqlRepositoryBase.Param("@Id", h.Id),
            SqlRepositoryBase.Param("@EliminadoPor", h.EliminadoPor)
        ];
    }
}