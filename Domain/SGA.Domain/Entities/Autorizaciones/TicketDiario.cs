namespace SGA.Domain.Entities.Autorizaciones
{
    public class TicketMensual : AutorizacionTransporte
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    [Obsolete("Usar TicketMensual. Se mantiene para compatibilidad con codigo anterior.")]
    public class TicketDiario : TicketMensual
    {
    }
}
