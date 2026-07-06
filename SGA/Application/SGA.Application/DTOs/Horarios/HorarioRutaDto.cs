namespace SGA.Application.DTOs.Horarios
{
    public sealed record HorarioRutaDto(
        int Id, int RutaId, TimeSpan HoraSalida, TimeSpan HoraLlegadaEstimada, bool Activo);

    public sealed record CrearHorarioRutaDto(
        int RutaId, TimeSpan HoraSalida, TimeSpan HoraLlegadaEstimada, string? CreadoPor);

    public sealed record ActualizarHorarioRutaDto(
        TimeSpan HoraSalida, TimeSpan HoraLlegadaEstimada, bool Activo);
}
