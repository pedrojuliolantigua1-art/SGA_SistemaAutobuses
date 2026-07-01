using SGA.Domain.Enum;

namespace SGA.Domain.Models.Autorizaciones
{
    public abstract class AutorizacionModel
    {
        public int Id { get; set; }
        public int UsuarioTransporteId { get; set; }
        public DateTime FechaEmision { get; set; }
        public EstadoAutorizacion Estado { get; set; } = EstadoAutorizacion.Activa;
    }
}