
namespace SGA.Domain.Models.Autorizaciones
{
    public class TicketMensualModel : AutorizacionModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}