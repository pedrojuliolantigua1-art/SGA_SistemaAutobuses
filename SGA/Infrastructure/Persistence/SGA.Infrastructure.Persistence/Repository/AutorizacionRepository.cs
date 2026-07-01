using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Enum;
using SGA.Domain.Models.Autorizaciones;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class AutorizacionRepository : SqlRepositoryBase, IAutorizacionRepository
    {
        public AutorizacionRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<AutorizacionModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_Autorizacion_GetById", AutorizacionMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<AutorizacionModel>> GetAllAsync()
            => await QueryAsync("sp_Autorizacion_GetAll", AutorizacionMapper.Map);

        public async Task<AutorizacionModel?> GetbyUsuario(int usuarioId)
            => await QuerySingleOrDefaultAsync("sp_Autorizacion_GetByUsuario", AutorizacionMapper.Map,Param("@UsuarioTransporteId", usuarioId));

        public async Task<IReadOnlyList<AutorizacionModel>> GetVigentes()
            => await QueryAsync("sp_Autorizacion_GetVigentes", AutorizacionMapper.Map);

        public async Task<IReadOnlyList<AutorizacionModel>> GetbyPeriodo(DateTime desde, DateTime hasta)
            => await QueryAsync("sp_Autorizacion_GetByPeriodo", AutorizacionMapper.Map,Param("@Desde", desde), Param("@Hasta", hasta));

        public async Task AddAsync(AutorizacionTransporte entity)
            => entity.Id = await ExecuteScalarAsync("sp_Autorizacion_Insert", AutorizacionParameters.ParaInsertar(entity));

        public async Task UpdateAsync(AutorizacionTransporte entity)
            => await ExecuteAsync("sp_Autorizacion_Update", AutorizacionParameters.ParaActualizar(entity));

        public async Task DeleteAsync(AutorizacionTransporte entity)
            => await ExecuteAsync("sp_Autorizacion_Delete", AutorizacionParameters.ParaEliminar(entity));

        public async Task<(int PagoId, int AutorizacionId)> EmitirAutorizacionAsync(
            int usuarioId, decimal monto, string tipoPago, string numeroComprobante,
            DateTime fechaHora, int registradoPorId, string tipoAutorizacion,
            DateTime fechaEmision, DateTime? fechaInicio, DateTime? fechaFin,
            string? numeroTarjeta, decimal? saldoInicial,
            string? condicionInstitucional, DateTime? fechaVencimiento, string creadoPor)
        {
            var resultado = await QuerySingleOrDefaultAsync(
                "sp_EmitirAutorizacion",
                r => (PagoId: r.Int("PagoId"), AutorizacionId: r.Int("AutorizacionId")),
                Param("@UsuarioTransporteId", usuarioId),
                Param("@Monto", monto),
                Param("@TipoPago", tipoPago),
                Param("@NumeroComprobante", numeroComprobante),
                Param("@FechaHoraPago", fechaHora),
                Param("@RegistradoPorUsuarioId", registradoPorId),
                Param("@TipoAutorizacion", tipoAutorizacion),
                Param("@FechaEmision", fechaEmision),
                Param("@FechaInicio", fechaInicio),
                Param("@FechaFin", fechaFin),
                Param("@NumeroTarjeta", numeroTarjeta),
                Param("@SaldoInicial", saldoInicial),
                Param("@CondicionInstitucional", condicionInstitucional),
                Param("@FechaVencimiento", fechaVencimiento),
                Param("@CreadoPor", creadoPor));

            return resultado;
        }
    }

    internal static class AutorizacionMapper
    {
        internal static AutorizacionModel Map(SqlReaderRow r)
        {
            var tipo = r.Str("TipoAutorizacion");
            var estado = r.Enum<EstadoAutorizacion>("Estado");

            AutorizacionModel model = tipo switch
            {
                "TicketMensual" => new TicketMensualModel
                {
                    FechaInicio = r.DateTime("FechaInicio"),
                    FechaFin = r.DateTime("FechaFin")
                },
                "TarjetaRecargable" => new TarjetaRecargableModel
                {
                    NumeroTarjeta = r.Str("NumeroTarjeta"),
                    SaldoDisponible = r.Dec("SaldoDisponible")
                },
                "PermisoTransporte" => new PermisoTransporteModel
                {
                    CondicionInstitucional = r.Str("CondicionInstitucional"),
                    FechaVencimiento = r.NullableDateTime("FechaVencimiento")
                },
                _ => throw new InvalidOperationException($"TipoAutorizacion desconocido: {tipo}")
            };

            model.Id = r.Int("Id");
            model.UsuarioTransporteId = r.Int("UsuarioTransporteId");
            model.FechaEmision = r.DateTime("FechaEmision");
            model.Estado = estado;
            return model;
        }
    }

    internal static class AutorizacionParameters
    {
        internal static SqlParameter[] ParaInsertar(AutorizacionTransporte a) =>
        [
            SqlRepositoryBase.Param("@UsuarioTransporteId", a.UsuarioTransporteId),
            SqlRepositoryBase.Param("@FechaEmision", a.FechaEmision),
            SqlRepositoryBase.Param("@Estado", (int)a.Estado),
            SqlRepositoryBase.Param("@TipoAutorizacion",ResolveTipo(a)),
            SqlRepositoryBase.Param("@FechaInicio", (a as TicketMensual)?.FechaInicio),
            SqlRepositoryBase.Param("@FechaFin",  (a as TicketMensual)?.FechaFin),
            SqlRepositoryBase.Param("@NumeroTarjeta", (a as TarjetaRecargable)?.NumeroTarjeta),
            SqlRepositoryBase.Param("@SaldoDisponible",  (a as TarjetaRecargable)?.SaldoDisponible),
            SqlRepositoryBase.Param("@CondicionInstitucional", (a as PermisoTransporte)?.CondicionInstitucional),
            SqlRepositoryBase.Param("@FechaVencimiento", (a as PermisoTransporte)?.FechaVencimiento),
            SqlRepositoryBase.Param("@CreadoPor", a.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(AutorizacionTransporte a) =>
        [
            SqlRepositoryBase.Param("@Id", a.Id),
            ..ParaInsertar(a)
        ];

        internal static SqlParameter[] ParaEliminar(AutorizacionTransporte a) =>
        [
            SqlRepositoryBase.Param("@Id",           a.Id),
            SqlRepositoryBase.Param("@EliminadoPor", a.EliminadoPor)
        ];

        private static string ResolveTipo(AutorizacionTransporte a) => a switch
        {
            TicketMensual => "TicketMensual",
            TarjetaRecargable => "TarjetaRecargable",
            PermisoTransporte => "PermisoTransporte",
            _ => throw new InvalidOperationException($"Tipo no soportado: {a.GetType().Name}")
        };
    }
}