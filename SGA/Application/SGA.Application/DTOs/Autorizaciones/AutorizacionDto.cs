using SGA.Domain.Enum;

namespace SGA.Application.DTOs.Autorizaciones
{
    public sealed record AutorizacionDto(
        int Id,
        int UsuarioTransporteId,
        string TipoAutorizacion,
        DateTime FechaEmision,
        EstadoAutorizacion Estado,
        DateTime? FechaInicio,
        DateTime? FechaFin,
        string? NumeroTarjeta,
        decimal? SaldoDisponible,
        string? CondicionInstitucional,
        DateTime? FechaVencimiento);

    public sealed record CrearTicketMensualDto(
        int UsuarioTransporteId,
        DateTime FechaInicio,
        string? CreadoPor);

    public sealed record CrearTarjetaRecargableDto(
        int UsuarioTransporteId,
        decimal SaldoInicial,
        string? NumeroTarjeta,
        string? CreadoPor);

    public sealed record CrearPermisoTransporteDto(
        int UsuarioTransporteId,
        string? CondicionInstitucional,
        DateTime? FechaVencimiento,
        string? CreadoPor);
}
