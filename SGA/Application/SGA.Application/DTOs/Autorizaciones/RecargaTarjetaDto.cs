namespace SGA.Application.DTOs.Autorizaciones
{
    public sealed record RecargaTarjetaDto(
        int Id, int TarjetaRecargableId, int PagoTransporteId, decimal Monto, DateTime FechaHora);

    public sealed record CrearRecargaTarjetaDto(
        int TarjetaRecargableId, decimal Monto, string TipoPago, string? NumeroComprobante, string? CreadoPor);
}
