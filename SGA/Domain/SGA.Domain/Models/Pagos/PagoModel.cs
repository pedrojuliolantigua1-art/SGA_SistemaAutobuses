namespace SGA.Domain.Models.Pagos
{
    using SGA.Domain.Enum;

    public class PagoModel
    {
        public int Id { get; set; }
        public int UsuarioTransporteId { get; set; }
        public int AutorizacionTransporteId { get; set; }
        public decimal Monto { get; set; }
        public string? TipoPago { get; set; }
        public EstadoPago Estado { get; set; }
        public string? NumeroComprobante { get; set; }
        public DateTime FechaHora { get; set; }
        public int RegistradoPorUsuarioId { get; set; }

        public string? UsuarioNombre { get; set; }
        public string? RegistradoPorNombre { get; set; }
    }
}
