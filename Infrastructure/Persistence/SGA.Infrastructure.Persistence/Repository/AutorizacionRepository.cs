using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Enum;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{

    public sealed class AutorizacionRepository : SqlRepositoryBase, IAutorizacionRepository
    {
        public AutorizacionRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<AutorizacionTransporte?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_Autorizacion_GetById", AutorizacionMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<AutorizacionTransporte>> GetAllAsync()
            => await QueryAsync("sp_Autorizacion_GetAll", AutorizacionMapper.Map);

        public async Task AddAsync(AutorizacionTransporte entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_Autorizacion_Insert",
                AutorizacionParameters.ParaInsertar(entity));

        public async Task UpdateAsync(AutorizacionTransporte entity)
            => await ExecuteAsync("sp_Autorizacion_Update",
                AutorizacionParameters.ParaActualizar(entity));

        public async Task DeleteAsync(AutorizacionTransporte entity)
            => await ExecuteAsync("sp_Autorizacion_Delete",
                AutorizacionParameters.ParaEliminar(entity));

        public async Task<AutorizacionTransporte> Getby_Usuario(int usuarioId)
        {
            var autorizacion = await QuerySingleOrDefaultAsync(
                "sp_Autorizacion_GetByUsuario", AutorizacionMapper.Map,
                Param("@UsuarioTransporteId", usuarioId));

            return autorizacion ?? throw new InvalidOperationException(
                $"No se encontro autorizacion activa para el usuario {usuarioId}.");
        }

        public async Task<IReadOnlyList<AutorizacionTransporte>> Get_Vigentes()
            => await QueryAsync("sp_Autorizacion_GetVigentes", AutorizacionMapper.Map);

        public async Task<IReadOnlyList<AutorizacionTransporte>> Geby_yPeriodo(
            DateTime desde, DateTime hasta)
            => await QueryAsync(
                "sp_Autorizacion_GetByPeriodo", AutorizacionMapper.Map,
                Param("@Desde", desde),
                Param("@Hasta", hasta));
    }


    internal static class AutorizacionMapper
    {
        internal static AutorizacionTransporte Map(SqlDataReader r)
        {
            var tipo = SqlRepositoryBase.GetString(r, "TipoAutorizacion");

            AutorizacionTransporte a = tipo switch
            {
                "TicketMensual" => new TicketMensual
                {
                    FechaInicio = SqlRepositoryBase.GetDateTime(r, "FechaInicio"),
                    FechaFin = SqlRepositoryBase.GetDateTime(r, "FechaFin")
                },
                "TarjetaRecargable" => new TarjetaRecargable
                {
                    NumeroTarjeta = SqlRepositoryBase.GetString(r, "NumeroTarjeta"),
                    SaldoDisponible = SqlRepositoryBase.GetDecimal(r, "SaldoDisponible")
                },
                "PermisoTransporte" => new PermisoTransporte
                {
                    CondicionInstitucional = SqlRepositoryBase.GetString(r, "CondicionInstitucional"),
                    FechaVencimiento = SqlRepositoryBase.GetNullableDateTime(r, "FechaVencimiento")
                },
                _ => throw new InvalidOperationException($"TipoAutorizacion desconocido: {tipo}")
            };

            a.Id = SqlRepositoryBase.GetInt(r, "Id");
            a.UsuarioTransporteId = SqlRepositoryBase.GetInt(r, "UsuarioTransporteId");
            a.FechaEmision = SqlRepositoryBase.GetDateTime(r, "FechaEmision");
            a.Estado = SqlRepositoryBase.GetEnum<EstadoAutorizacion>(r, "Estado");
            a.FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion");
            a.CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor");
            a.FechaModificacion = SqlRepositoryBase.GetDateTime(r, "FechaModificacion");
            a.Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado");
            a.EliminadoPor = SqlRepositoryBase.GetString(r, "EliminadoPor");

            return a;
        }
    }

    internal static class AutorizacionParameters
    {
        internal static SqlParameter[] ParaInsertar(AutorizacionTransporte a) =>
        [
            SqlRepositoryBase.Param("@UsuarioTransporteId",    a.UsuarioTransporteId),
            SqlRepositoryBase.Param("@FechaEmision",           a.FechaEmision),
            SqlRepositoryBase.Param("@Estado",                 (int)a.Estado),
            SqlRepositoryBase.Param("@TipoAutorizacion",       ResolveTipo(a)),
            SqlRepositoryBase.Param("@FechaInicio",            (a as TicketMensual)?.FechaInicio),
            SqlRepositoryBase.Param("@FechaFin",               (a as TicketMensual)?.FechaFin),
            SqlRepositoryBase.Param("@NumeroTarjeta",          (a as TarjetaRecargable)?.NumeroTarjeta),
            SqlRepositoryBase.Param("@SaldoDisponible",        (a as TarjetaRecargable)?.SaldoDisponible),
            SqlRepositoryBase.Param("@CondicionInstitucional", (a as PermisoTransporte)?.CondicionInstitucional),
            SqlRepositoryBase.Param("@FechaVencimiento",       (a as PermisoTransporte)?.FechaVencimiento),
            SqlRepositoryBase.Param("@CreadoPor",              a.CreadoPor)
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
            _ => throw new InvalidOperationException(
                     $"Tipo no soportado: {a.GetType().Name}")
        };
    }
}
