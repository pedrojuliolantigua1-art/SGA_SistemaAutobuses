
namespace SGA.Domain.Models.Autorizaciones
{
    public class RecargaTarjetaModel
    {
        public int Id { get; set; }
        public int TarjetaRecargableId { get; set; }
        public int PagoTransporteId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaHora { get; set; }
    }
}