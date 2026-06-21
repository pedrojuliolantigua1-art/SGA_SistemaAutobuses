using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Conexion;

namespace SGA.Infrastructure.Persistence.Repositories
{
 
    public sealed class AuditoriaRepository : SqlRepositoryBase, IAuditoriaRepository
    {
        public AuditoriaRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<RegistroAuditoria?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_Auditoria_GetById", AuditoriaMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<RegistroAuditoria>> GetAllAsync()
            => await QueryAsync("sp_Auditoria_GetAll", AuditoriaMapper.Map);

        public async Task AddAsync(RegistroAuditoria entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_Auditoria_Insert",
                AuditoriaParameters.ParaInsertar(entity));

        public Task UpdateAsync(RegistroAuditoria entity)
            => throw new InvalidOperationException(
                "Los registros de auditoria son inmutables.");

        public Task DeleteAsync(RegistroAuditoria entity)
            => throw new InvalidOperationException(
                "Los registros de auditoria no se pueden eliminar.");

        public async Task<IReadOnlyList<RegistroAuditoria>> Getby_Periodo(
            DateTime desde, DateTime hasta)
            => await QueryAsync(
                "sp_Auditoria_GetByPeriodo", AuditoriaMapper.Map,
                Param("@Desde", desde),
                Param("@Hasta", hasta));

        public async Task<IReadOnlyList<RegistroAuditoria>> GetBy_Actor(int usuarioId)
            => await QueryAsync(
                "sp_Auditoria_GetByActor", AuditoriaMapper.Map,
                Param("@UsuarioTransporteId", usuarioId));

        public async Task<IReadOnlyList<RegistroAuditoria>> Getby_Accion(string accion)
            => await QueryAsync(
                "sp_Auditoria_GetByAccion", AuditoriaMapper.Map,
                Param("@Accion", accion));
    }

    internal static class AuditoriaMapper
    {
        internal static RegistroAuditoria Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            UsuarioTransporteId = SqlRepositoryBase.GetInt(r, "UsuarioTransporteId"),
            Accion = SqlRepositoryBase.GetString(r, "Accion"),
            EntidadAfectada = SqlRepositoryBase.GetString(r, "EntidadAfectada"),
            EntidadId = SqlRepositoryBase.GetString(r, "EntidadId"),
            Detalle = SqlRepositoryBase.GetString(r, "Detalle"),
            FechaHora = SqlRepositoryBase.GetDateTime(r, "FechaHora"),
            FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion"),
            CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor"),
            Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado")
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
