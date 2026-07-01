using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Notificaciones;
using SGA.Domain.Models.Notificaciones;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class NotificacionRepository : SqlRepositoryBase, INotificacionRepository
    {
        public NotificacionRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<NotificacionModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_Notificacion_GetById", NotificacionMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<NotificacionModel>> GetAllAsync()
            => await QueryAsync("sp_Notificacion_GetAll", NotificacionMapper.Map);

        public async Task<IReadOnlyList<NotificacionModel>> GetByUsuario(int usuarioId)
            => await QueryAsync("sp_Notificacion_GetByUsuario", NotificacionMapper.Map,
                Param("@UsuarioTransporteId", usuarioId));

        public async Task<IReadOnlyList<NotificacionModel>> GetByPeriodo(DateTime desde, DateTime hasta)
            => await QueryAsync("sp_Notificacion_GetByPeriodo", NotificacionMapper.Map,
                Param("@Desde", desde), Param("@Hasta", hasta));

        public async Task<IReadOnlyList<NotificacionModel>> GetByTipo(string tipo)
            => await QueryAsync("sp_Notificacion_GetByTipo", NotificacionMapper.Map, Param("@Tipo", tipo));

        public async Task AddAsync(Notificacion entity)
            => entity.Id = await ExecuteScalarAsync("sp_Notificacion_Insert", NotificacionParameters.ParaInsertar(entity));

        public async Task UpdateAsync(Notificacion entity)
            => await ExecuteAsync("sp_Notificacion_Update", NotificacionParameters.ParaActualizar(entity));

        public async Task DeleteAsync(Notificacion entity)
            => await ExecuteAsync("sp_Notificacion_Delete", NotificacionParameters.ParaEliminar(entity));

        public async Task MarcarComoLeida(int notificacionId)
            => await ExecuteAsync("sp_Notificacion_MarcarLeida", Param("@Id", notificacionId));
    }

    internal static class NotificacionMapper
    {
        internal static NotificacionModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            UsuarioTransporteId = r.Int("UsuarioTransporteId"),
            Tipo = r.Str("Tipo") ?? string.Empty,
            Titulo = r.Str("Titulo") ?? string.Empty,
            Mensaje = r.Str("Mensaje") ?? string.Empty,
            FechaHora = r.DateTime("FechaHora"),
            Leida = r.Bool("Leida")
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