using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Notificaciones;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;
using SGA.Infrastructure.Persistence.Conexion;

namespace SGA.Infrastructure.Persistence.Repositories
{
    
    public sealed class NotificacionRepository : SqlRepositoryBase, INotificacionRepository
    {
        public NotificacionRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<Notificacion?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_Notificacion_GetById", NotificacionMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<Notificacion>> GetAllAsync()
            => await QueryAsync("sp_Notificacion_GetAll", NotificacionMapper.Map);

        public async Task AddAsync(Notificacion entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_Notificacion_Insert",
                NotificacionParameters.ParaInsertar(entity));

        public async Task UpdateAsync(Notificacion entity)
            => await ExecuteAsync("sp_Notificacion_Update",
                NotificacionParameters.ParaActualizar(entity));

        public async Task DeleteAsync(Notificacion entity)
            => await ExecuteAsync("sp_Notificacion_Delete",
                NotificacionParameters.ParaEliminar(entity));

        public async Task<IReadOnlyList<Notificacion>> GetBy_Usuario(int usuarioId)
            => await QueryAsync(
                "sp_Notificacion_GetByUsuario", NotificacionMapper.Map,
                Param("@UsuarioTransporteId", usuarioId));

        public async Task<IReadOnlyList<Notificacion>> GetBy_Periodo(
            DateTime desde, DateTime hasta)
            => await QueryAsync(
                "sp_Notificacion_GetByPeriodo", NotificacionMapper.Map,
                Param("@Desde", desde),
                Param("@Hasta", hasta));

        public async Task<IReadOnlyList<Notificacion>> GetBy_Tipo(string tipo)
            => await QueryAsync(
                "sp_Notificacion_GetByTipo", NotificacionMapper.Map,
                Param("@Tipo", tipo));

        public async Task MarcarComoLeida(int notificacionId)
            => await ExecuteAsync("sp_Notificacion_MarcarLeida",
                Param("@Id", notificacionId));
    }

    internal static class NotificacionMapper
    {
        internal static Notificacion Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            UsuarioTransporteId = SqlRepositoryBase.GetInt(r, "UsuarioTransporteId"),
            Tipo = SqlRepositoryBase.GetString(r, "Tipo") ?? string.Empty,
            Titulo = SqlRepositoryBase.GetString(r, "Titulo") ?? string.Empty,
            Mensaje = SqlRepositoryBase.GetString(r, "Mensaje") ?? string.Empty,
            FechaHora = SqlRepositoryBase.GetDateTime(r, "FechaHora"),
            Leida = SqlRepositoryBase.GetBool(r, "Leida"),
            FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion"),
            CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor"),
            FechaModificacion = SqlRepositoryBase.GetDateTime(r, "FechaModificacion"),
            Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado"),
            EliminadoPor = SqlRepositoryBase.GetString(r, "EliminadoPor")
        };
    }

    internal static class NotificacionParameters
    {
        internal static SqlParameter[] ParaInsertar(Notificacion n) =>
        [
            SqlRepositoryBase.Param("@UsuarioTransporteId", n.UsuarioTransporteId),
            SqlRepositoryBase.Param("@Tipo",                n.Tipo),
            SqlRepositoryBase.Param("@Titulo",              n.Titulo),
            SqlRepositoryBase.Param("@Mensaje",             n.Mensaje),
            SqlRepositoryBase.Param("@FechaHora",           n.FechaHora),
            SqlRepositoryBase.Param("@Leida",               n.Leida),
            SqlRepositoryBase.Param("@CreadoPor",           n.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(Notificacion n) =>
        [
            SqlRepositoryBase.Param("@Id",    n.Id),
            SqlRepositoryBase.Param("@Leida", n.Leida)
        ];

        internal static SqlParameter[] ParaEliminar(Notificacion n) =>
        [
            SqlRepositoryBase.Param("@Id",           n.Id),
            SqlRepositoryBase.Param("@EliminadoPor", n.EliminadoPor)
        ];
    }
}
