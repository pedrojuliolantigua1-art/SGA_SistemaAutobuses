using SGA.Application.DTOs.Paradas;
using SGA.Application.DTOs.Horarios;

namespace SGA.Application.DTOs.Rutas
{
    public sealed record RutaDto(
        int Id, string? Nombre, string? Descripcion, bool Activa);

    public sealed record CrearRutaDto(
        string? Nombre, string? Descripcion, bool Activa, string? CreadoPor);

    public sealed record ActualizarRutaDto(
        string? Nombre, string? Descripcion, bool Activa);

    public sealed record RutaDetalleDto(
        RutaDto Ruta,
        IReadOnlyList<ParadaDto> Paradas,
        IReadOnlyList<HorarioRutaDto> Horarios);
}
