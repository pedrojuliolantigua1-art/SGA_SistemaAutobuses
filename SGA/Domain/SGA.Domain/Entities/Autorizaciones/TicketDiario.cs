namespace SGA.Domain.Entities.Autorizaciones
{
    public class TicketMensual : AutorizacionTransporte
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

}
