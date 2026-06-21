using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{

    public sealed class PagoRepository : SqlRepositoryBase, IPagoRepository
    {
        public PagoRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<PagoTransporte?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_Pago_GetById", PagoMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<PagoTransporte>> GetAllAsync()
            => await QueryAsync("sp_Pago_GetAll", PagoMapper.Map);

        public async Task AddAsync(PagoTransporte entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_Pago_Insert",
                PagoParameters.ParaInsertar(entity));

        public async Task UpdateAsync(PagoTransporte entity)
            => await ExecuteAsync("sp_Pago_Update",
                PagoParameters.ParaActualizar(entity));

        public async Task DeleteAsync(PagoTransporte entity)
            => await ExecuteAsync("sp_Pago_Delete",
                PagoParameters.ParaEliminar(entity));

        public async Task<IReadOnlyList<PagoTransporte>> GetBy_Usuario(int usuarioId)
            => await QueryAsync(
                "sp_Pago_GetByUsuario", PagoMapper.Map,
                Param("@UsuarioTransporteId", usuarioId));

        public async Task<PagoTransporte?> Get_PagoSinAutorizacion(int usuarioId)
            => await QuerySingleOrDefaultAsync(
                "sp_Pago_GetSinAutorizacion", PagoMapper.Map,
                Param("@UsuarioTransporteId", usuarioId));
    }

    internal static class PagoMapper
    {
        internal static PagoTransporte Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            UsuarioTransporteId = SqlRepositoryBase.GetInt(r, "UsuarioTransporteId"),
            AutorizacionTransporteId = SqlRepositoryBase.GetInt(r, "AutorizacionTransporteId"),
            Monto = SqlRepositoryBase.GetDecimal(r, "Monto"),
            TipoPago = SqlRepositoryBase.GetString(r, "TipoPago"),
            Estado = SqlRepositoryBase.GetString(r, "Estado"),
            NumeroComprobante = SqlRepositoryBase.GetString(r, "NumeroComprobante"),
            FechaHora = SqlRepositoryBase.GetDateTime(r, "FechaHora"),
            RegistradoPorUsuarioId = SqlRepositoryBase.GetInt(r, "RegistradoPorUsuarioId"),
            FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion"),
            CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor"),
            FechaModificacion = SqlRepositoryBase.GetDateTime(r, "FechaModificacion"),
            Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado"),
            EliminadoPor = SqlRepositoryBase.GetString(r, "EliminadoPor")
        };
    }

    internal static class PagoParameters
    {
        internal static SqlParameter[] ParaInsertar(PagoTransporte p) =>
        [
            SqlRepositoryBase.Param("@UsuarioTransporteId",     p.UsuarioTransporteId),
            SqlRepositoryBase.Param("@AutorizacionTransporteId",p.AutorizacionTransporteId),
            SqlRepositoryBase.Param("@Monto",                   p.Monto),
            SqlRepositoryBase.Param("@TipoPago",                p.TipoPago),
            SqlRepositoryBase.Param("@Estado",                  p.Estado),
            SqlRepositoryBase.Param("@NumeroComprobante",       p.NumeroComprobante),
            SqlRepositoryBase.Param("@FechaHora",               p.FechaHora),
            SqlRepositoryBase.Param("@RegistradoPorUsuarioId",  p.RegistradoPorUsuarioId),
            SqlRepositoryBase.Param("@CreadoPor",               p.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(PagoTransporte p) =>
        [
            SqlRepositoryBase.Param("@Id",     p.Id),
            SqlRepositoryBase.Param("@Estado", p.Estado)
        ];

        internal static SqlParameter[] ParaEliminar(PagoTransporte p) =>
        [
            SqlRepositoryBase.Param("@Id",           p.Id),
            SqlRepositoryBase.Param("@EliminadoPor", p.EliminadoPor)
        ];
    }
}
