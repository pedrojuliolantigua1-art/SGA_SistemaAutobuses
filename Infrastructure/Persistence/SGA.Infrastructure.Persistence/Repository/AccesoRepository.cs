using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Accesos;
using SGA.Domain.Enum;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{

    public sealed class AccesoRepository : SqlRepositoryBase, IAccesoRepository
    {
        public AccesoRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<RegistroUsoTransporte?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_Acceso_GetById", AccesoMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<RegistroUsoTransporte>> GetAllAsync()
            => await QueryAsync("sp_Acceso_GetAll", AccesoMapper.Map);

        public async Task AddAsync(RegistroUsoTransporte entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_Acceso_Insert",
                AccesoParameters.ParaInsertar(entity));

        public Task UpdateAsync(RegistroUsoTransporte entity)
            => throw new InvalidOperationException(
                "Los registros de acceso son inmutables.");

        public Task DeleteAsync(RegistroUsoTransporte entity)
            => throw new InvalidOperationException(
                "Los registros de acceso no se pueden eliminar.");

        public async Task<IReadOnlyList<RegistroUsoTransporte>> GetBy_Viaje(int viajeId)
            => await QueryAsync(
                "sp_Acceso_GetByViaje", AccesoMapper.Map,
                Param("@ViajeId", viajeId));

        public async Task<IReadOnlyList<RegistroUsoTransporte>> GetBy_Usuario(int usuarioId)
            => await QueryAsync(
                "sp_Acceso_GetByUsuario", AccesoMapper.Map,
                Param("@UsuarioTransporteId", usuarioId));

        public async Task<IReadOnlyList<RegistroUsoTransporte>> GetBy_Periodo(
            DateTime desde, DateTime hasta)
            => await QueryAsync(
                "sp_Acceso_GetByPeriodo", AccesoMapper.Map,
                Param("@Desde", desde),
                Param("@Hasta", hasta));
    }

    internal static class AccesoMapper
    {
        internal static RegistroUsoTransporte Map(SqlDataReader r) => new()
        {
            Id = SqlRepositoryBase.GetInt(r, "Id"),
            UsuarioTransporteId = SqlRepositoryBase.GetInt(r, "UsuarioTransporteId"),
            ViajeId = SqlRepositoryBase.GetInt(r, "ViajeId"),
            AutorizacionTransporteId = SqlRepositoryBase.GetNullableInt(r, "AutorizacionTransporteId"),
            ResultadoAcceso = SqlRepositoryBase.GetEnum<ResultadoAcceso>(r, "ResultadoAcceso"),
            MotivoRechazo = SqlRepositoryBase.GetString(r, "MotivoRechazo"),
            FechaHora = SqlRepositoryBase.GetDateTime(r, "FechaHora"),
            ValidadoPorUsuarioId = SqlRepositoryBase.GetInt(r, "ValidadoPorUsuarioId"),
            FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion"),
            CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor"),
            Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado")
        };
    }

    internal static class AccesoParameters
    {
        internal static SqlParameter[] ParaInsertar(RegistroUsoTransporte e) =>
        [
            SqlRepositoryBase.Param("@UsuarioTransporteId",      e.UsuarioTransporteId),
            SqlRepositoryBase.Param("@ViajeId",                  e.ViajeId),
            SqlRepositoryBase.Param("@AutorizacionTransporteId", e.AutorizacionTransporteId),
            SqlRepositoryBase.Param("@ResultadoAcceso",          (int)e.ResultadoAcceso),
            SqlRepositoryBase.Param("@MotivoRechazo",            e.MotivoRechazo),
            SqlRepositoryBase.Param("@FechaHora",                e.FechaHora),
            SqlRepositoryBase.Param("@ValidadoPorUsuarioId",     e.ValidadoPorUsuarioId),
            SqlRepositoryBase.Param("@CreadoPor",                e.CreadoPor)
        ];
    }
}
