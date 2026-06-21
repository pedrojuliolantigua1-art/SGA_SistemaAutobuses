namespace SGA.Domain.Entities.Autorizaciones
{
    public class TicketDiario : AutorizacionTransporte
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

}
