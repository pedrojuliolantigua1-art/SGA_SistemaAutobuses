namespace SGA.Application.DTOs.Autobuses
{
    public sealed record AutobusDto(
        int Id, string? Placa, string? Marca, string? Modelo, int Capacidad, string Estado);

    public sealed record CrearAutobusDto(
        string Placa, string Marca, string Modelo, int Capacidad, string? CreadoPor);

    public sealed record ActualizarAutobusDto(
        string Placa, string Marca, string Modelo, int Capacidad);

    public sealed record CambiarEstadoAutobusDto(
        string NuevoEstado);
}
