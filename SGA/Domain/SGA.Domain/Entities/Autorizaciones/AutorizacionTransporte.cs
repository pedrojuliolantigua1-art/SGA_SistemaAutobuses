using SGA.Domain.Base;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Enum;

namespace SGA.Domain.Entities.Autorizaciones
{
    public abstract class AutorizacionTransporte : BaseEntity
    {
        public int UsuarioTransporteId { get; set; }
        public DateTime FechaEmision { get; set; }
        public EstadoAutorizacion Estado { get; set; } = EstadoAutorizacion.Activa;

        public virtual UsuarioTransporte? Usuario { get; set; }
    }
}
