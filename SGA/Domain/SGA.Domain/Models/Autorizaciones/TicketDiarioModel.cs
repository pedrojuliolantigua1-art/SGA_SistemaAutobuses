
namespace SGA.Domain.Models.Autorizaciones
{
    public class TicketDiarioModel : AutorizacionModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}