using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Models.Auditoria;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class AuditoriaRepository : SqlRepositoryBase, IAuditoriaRepository
    {
        public AuditoriaRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<AuditoriaModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_Auditoria_GetById", AuditoriaMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<AuditoriaModel>> GetAllAsync()
            => await QueryAsync("sp_Auditoria_GetAll", AuditoriaMapper.Map);

        public async Task<IReadOnlyList<AuditoriaModel>> GetbyPeriodo(DateTime desde, DateTime hasta)
            => await QueryAsync("sp_Auditoria_GetByPeriodo", AuditoriaMapper.Map,
                Param("@Desde", desde), Param("@Hasta", hasta));

        public async Task<IReadOnlyList<AuditoriaModel>> GetByActor(int usuarioId)
            => await QueryAsync("sp_Auditoria_GetByActor", AuditoriaMapper.Map,
                Param("@UsuarioTransporteId", usuarioId));

        public async Task<IReadOnlyList<AuditoriaModel>> GetbyAccion(string accion)
            => await QueryAsync("sp_Auditoria_GetByAccion", AuditoriaMapper.Map, Param("@Accion", accion));

        public async Task AddAsync(RegistroAuditoria entity)
            => entity.Id = await ExecuteScalarAsync("sp_Auditoria_Insert", AuditoriaParameters.ParaInsertar(entity));

        public Task UpdateAsync(RegistroAuditoria entity)
            => throw new InvalidOperationException("Los registros de auditoria son inmutables.");

        public Task DeleteAsync(RegistroAuditoria entity)
            => throw new InvalidOperationException("Los registros de auditoria no se pueden eliminar.");
    }

    internal static class AuditoriaMapper
    {
        internal static AuditoriaModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            UsuarioTransporteId = r.Int("UsuarioTransporteId"),
            Accion = r.Str("Accion"),
            EntidadAfectada = r.Str("EntidadAfectada"),
            EntidadId = r.Str("EntidadId"),
            Detalle = r.Str("Detalle"),
            FechaHora = r.DateTime("FechaHora")
        };
    }

    internal static class AuditoriaParameters
    {
        internal static SqlParameter[] ParaInsertar(RegistroAuditoria a) =>
        [
            SqlRepositoryBase.Param("@UsuarioTransporteId", a.UsuarioTransporteId),
            SqlRepositoryBase.Param("@Accion",              a.Accion),
            SqlRepositoryBase.Param("@EntidadAfectada",     a.EntidadAfectada),
            SqlRepositoryBase.Param("@EntidadId",           a.EntidadId),
            SqlRepositoryBase.Param("@Detalle",             a.Detalle),
            SqlRepositoryBase.Param("@FechaHora",           a.FechaHora),
            SqlRepositoryBase.Param("@CreadoPor",           a.CreadoPor)
        ];
    }
}