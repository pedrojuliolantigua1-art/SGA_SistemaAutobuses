namespace SGA.Application.DTOs.Rutas
{
    public sealed record RutaDto(
        int Id,
        string? Nombre,
        string? Descripcion,
        bool Activa);

    public sealed record ParadaDto(
        int Id,
        int RutaId,
        string? Nombre,
        string? Referencia,
        int Orden);

    public sealed record HorarioRutaDto(
        int Id,
        int RutaId,
        TimeSpan HoraSalida,
        TimeSpan HoraLlegadaEstimada,
        bool Activo);

    public sealed record RutaDetalleDto(
        RutaDto Ruta,
        IReadOnlyList<ParadaDto> Paradas,
        IReadOnlyList<HorarioRutaDto> Horarios);

    public sealed record CrearRutaDto(
        string? Nombre,
        string? Descripcion,
        bool Activa,
        string? CreadoPor);

    public sealed record ActualizarRutaDto(
        int Id,
        string? Nombre,
        string? Descripcion,
        bool Activa);
}
