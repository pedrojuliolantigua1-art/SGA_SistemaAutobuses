using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Accesos;
using SGA.Domain.Enum;
using SGA.Domain.Models.Accesos;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class AccesoRepository : SqlRepositoryBase, IAccesoRepository
    {
        public AccesoRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<AccesoModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_Acceso_GetById", AccesoMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<AccesoModel>> GetAllAsync()
            => await QueryAsync("sp_Acceso_GetAll", AccesoMapper.Map);

        public async Task<IReadOnlyList<AccesoModel>> GetByViaje(int viajeId)
            => await QueryAsync("sp_Acceso_GetByViaje", AccesoMapper.Map, Param("@ViajeId", viajeId));

        public async Task<IReadOnlyList<AccesoModel>> GetByUsuario(int usuarioId)
            => await QueryAsync("sp_Acceso_GetByUsuario", AccesoMapper.Map, Param("@UsuarioTransporteId", usuarioId));

        public async Task<IReadOnlyList<AccesoModel>> GetByPeriodo(DateTime desde, DateTime hasta)
            => await QueryAsync("sp_Acceso_GetByPeriodo", AccesoMapper.Map,
                Param("@Desde", desde), Param("@Hasta", hasta));

        public async Task AddAsync(RegistroUsoTransporte entity)
            => entity.Id = await ExecuteScalarAsync("sp_Acceso_Insert", AccesoParameters.ParaInsertar(entity));

        public Task UpdateAsync(RegistroUsoTransporte entity)
            => throw new InvalidOperationException("Los registros de acceso son inmutables.");

        public Task DeleteAsync(RegistroUsoTransporte entity)
            => throw new InvalidOperationException("Los registros de acceso no se pueden eliminar.");

        public async Task<int> RegistrarAbordajeAsync(
            int usuarioId, int viajeId, int autorizacionId, int resultadoAcceso,
            string? motivoRechazo, DateTime fechaHora, int validadorId,
            string creadoPor, decimal costoViaje = 1.00m)
            => await ExecuteScalarAsync("sp_ValidarAbordaje",
                Param("@UsuarioTransporteId", usuarioId),
                Param("@ViajeId", viajeId),
                Param("@AutorizacionTransporteId", autorizacionId),
                Param("@ResultadoAcceso", resultadoAcceso),
                Param("@MotivoRechazo", motivoRechazo),
                Param("@FechaHora", fechaHora),
                Param("@ValidadoPorUsuarioId", validadorId),
                Param("@CreadoPor", creadoPor),
                Param("@CostoViaje", costoViaje));
    }

    internal static class AccesoMapper
    {
        internal static AccesoModel Map(SqlReaderRow r) => new()
        {
            Id = r.Int("Id"),
            UsuarioTransporteId = r.Int("UsuarioTransporteId"),
            ViajeId = r.Int("ViajeId"),
            AutorizacionTransporteId = r.NullableInt("AutorizacionTransporteId"),
            ResultadoAcceso = r.Enum<ResultadoAcceso>("ResultadoAcceso"),
            MotivoRechazo = r.Str("MotivoRechazo"),
            FechaHora = r.DateTime("FechaHora"),
            ValidadoPorUsuarioId = r.Int("ValidadoPorUsuarioId")
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