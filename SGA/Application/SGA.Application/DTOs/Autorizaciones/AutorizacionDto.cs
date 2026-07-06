using SGA.Domain.Enum;

namespace SGA.Application.DTOs.Autorizaciones
{
    public sealed record AutorizacionResumenDto(
        int Id, int UsuarioTransporteId, string TipoAutorizacion,
        DateTime FechaEmision, EstadoAutorizacion Estado);

    public sealed record TicketDiarioDto(
        int Id, int UsuarioTransporteId, DateTime FechaEmision, EstadoAutorizacion Estado,
        DateTime FechaInicio, DateTime FechaFin);

    public sealed record CrearTicketDiarioDto(
        int UsuarioTransporteId, DateTime FechaInicio, DateTime FechaFin, string? CreadoPor);

    public sealed record TarjetaRecargableDto(
        int Id, int UsuarioTransporteId, DateTime FechaEmision, EstadoAutorizacion Estado,
        string? NumeroTarjeta, decimal SaldoDisponible);

    public sealed record CrearTarjetaRecargableDto(
        int UsuarioTransporteId, decimal SaldoInicial, string? NumeroTarjeta, string? CreadoPor);

    public sealed record TarjetaRecargableDetalleDto(
        TarjetaRecargableDto Tarjeta, IReadOnlyList<RecargaTarjetaDto> Historial);

    public sealed record PermisoTransporteDto(
        int Id, int UsuarioTransporteId, DateTime FechaEmision, EstadoAutorizacion Estado,
        string? CondicionInstitucional, DateTime? FechaVencimiento);

    public sealed record CrearPermisoTransporteDto(
        int UsuarioTransporteId, string? CondicionInstitucional, DateTime? FechaVencimiento, string? CreadoPor);

    public sealed record AnularAutorizacionDto(int Id, string? Motivo, string? AnuladoPor);
}
