using SGA.Domain.Enum;

namespace SGA.Application.DTOs.Viajes
{
    public sealed record ViajeDto(
        int Id, int RutaId, int HorarioRutaId, int AutobusId, int ConductorId,
        DateTime Fecha, EstadoViaje Estado, DateTime? HoraInicioReal, DateTime? HoraFinReal,
        int CupoActual, int CapacidadMaxima);

    public sealed record ProgramarViajeDto(
        int RutaId, int HorarioRutaId, int AutobusId, int ConductorId,
        DateTime Fecha, string? CreadoPor);

    public sealed record EjecutarViajeDto(
        int ViajeId, int ConductorId, DateTime FechaHora);

    public sealed record CancelarViajeDto(
        int ViajeId, string Motivo, string? CreadoPor);

    public sealed record IncidenciaDto(
        int Id, int ViajeId, int ConductorId, string? Tipo, string? Descripcion, DateTime FechaHora);

    public sealed record ReportarIncidenciaDto(
        int ViajeId, int ConductorId, string Tipo, string Descripcion,
        DateTime FechaHora, string? CreadoPor);
}
