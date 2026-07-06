namespace SGA.Application.DTOs.Paradas
{
    public sealed record ParadaDto(
        int Id, int RutaId, string? Nombre, string? Referencia, int Orden);

    public sealed record CrearParadaDto(
        int RutaId, string Nombre, string? Referencia, int Orden, string? CreadoPor);

    public sealed record ActualizarParadaDto(
        string Nombre, string? Referencia, int Orden);

    public sealed record OrdenParadaDto(int ParadaId, int NuevoOrden);

    public sealed record ReordenarParadasDto(
        int RutaId, IReadOnlyList<OrdenParadaDto> Orden);
}
