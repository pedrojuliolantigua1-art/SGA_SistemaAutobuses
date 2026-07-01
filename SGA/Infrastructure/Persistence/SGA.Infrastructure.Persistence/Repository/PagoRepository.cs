using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Models.Pagos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class PagoRepository : SqlRepositoryBase, IPagoRepository
    {
        public PagoRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<PagoModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_Pago_GetById", PagoMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<PagoModel>> GetAllAsync()
            => await QueryAsync("sp_Pago_GetAll", PagoMapper.Map);

        public async Task<IReadOnlyList<PagoModel>> GetByUsuario(int usuarioId)
            => await QueryAsync("sp_Pago_GetByUsuario", PagoMapper.Map, Param("@UsuarioTransporteId", usuarioId));

        public async Task<PagoModel?> GetPagoSinAutorizacion(int usuarioId)
            => await QuerySingleOrDefaultAsync("sp_Pago_GetSinAutorizacion", PagoMapper.Map,
                Param("@UsuarioTransporteId", usuarioId));

        public async Task AddAsync(PagoTransporte entity)
            => entity.Id = await ExecuteScalarAsync("sp_Pago_Insert", PagoParameters.ParaInsertar(entity));

        public async Task UpdateAsync(PagoTransporte entity)
            => await ExecuteAsync("sp_Pago_Update", PagoParameters.ParaActualizar(entity));

        public async Task DeleteAsync(PagoTransporte entity)
            => await ExecuteAsync("sp_Pago_Delete", PagoParameters.ParaEliminar(entity));
    }

    internal static class PagoMapper
    {
        internal static PagoModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            UsuarioTransporteId = r.Int("UsuarioTransporteId"),
            AutorizacionTransporteId = r.Int("AutorizacionTransporteId"),
            Monto = r.Dec("Monto"),
            TipoPago = r.Str("TipoPago"),
            Estado = r.Str("Estado"),
            NumeroComprobante = r.Str("NumeroComprobante"),
            FechaHora = r.DateTime("FechaHora"),
            RegistradoPorUsuarioId = r.Int("RegistradoPorUsuarioId")
        };
    }

    internal static class PagoParameters
    {
        internal static SqlParameter[] ParaInsertar(PagoTransporte p) =>
        [
            SqlRepositoryBase.Param("@UsuarioTransporteId", p.UsuarioTransporteId),
            SqlRepositoryBase.Param("@AutorizacionTransporteId", p.AutorizacionTransporteId),
            SqlRepositoryBase.Param("@Monto",p.Monto),
            SqlRepositoryBase.Param("@TipoPago",p.TipoPago),
            SqlRepositoryBase.Param("@Estado", p.Estado),
            SqlRepositoryBase.Param("@NumeroComprobante",p.NumeroComprobante),
            SqlRepositoryBase.Param("@FechaHora", p.FechaHora),
            SqlRepositoryBase.Param("@RegistradoPorUsuarioId", p.RegistradoPorUsuarioId),
            SqlRepositoryBase.Param("@CreadoPor", p.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(PagoTransporte p) =>
        [
            SqlRepositoryBase.Param("@Id", p.Id),
            SqlRepositoryBase.Param("@Estado", p.Estado)
        ];

        internal static SqlParameter[] ParaEliminar(PagoTransporte p) =>
        [
            SqlRepositoryBase.Param("@Id",  p.Id),
            SqlRepositoryBase.Param("@EliminadoPor", p.EliminadoPor)
        ];
    }
}