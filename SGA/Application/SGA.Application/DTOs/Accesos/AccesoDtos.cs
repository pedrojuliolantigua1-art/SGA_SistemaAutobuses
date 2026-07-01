using SGA.Domain.Enum;

namespace SGA.Application.DTOs.Accesos
{
    public sealed record AccesoDto(
        int Id,
        int UsuarioTransporteId,
        int ViajeId,
        int? AutorizacionTransporteId,
        ResultadoAcceso ResultadoAcceso,
        string? MotivoRechazo,
        DateTime FechaHora,
        int ValidadoPorUsuarioId);

    public sealed record RegistrarAccesoDto(
        int UsuarioTransporteId,
        int ViajeId,
        int ValidadoPorUsuarioId,
        DateTime FechaHora,
        decimal CostoViaje = 1m,
        string? CreadoPor = null);
}
