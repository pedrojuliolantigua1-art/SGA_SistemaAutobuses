namespace SGA.Application.DTOs.Fotos
{
    public sealed record FotoAutobusDto(
        int Id, int AutobusId, string NombreArchivo, string UrlPublica, DateTime FechaSubida);

    public sealed record SubirFotoAutobusDto(
        int AutobusId, string NombreArchivo, string UrlPublica, string PublicId, string? SubidoPor);

    public sealed record FotoIncidenciaDto(
        int Id, int IncidenciaId, string NombreArchivo, string UrlPublica, DateTime FechaSubida);

    public sealed record SubirFotoIncidenciaDto(
        int IncidenciaId, string NombreArchivo, string UrlPublica, string PublicId, string? SubidoPor);
}
