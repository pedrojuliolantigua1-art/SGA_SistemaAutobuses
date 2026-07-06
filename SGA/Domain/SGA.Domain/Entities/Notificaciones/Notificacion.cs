using SGA.Domain.Base;
using SGA.Domain.Entities.Usuarios;

namespace SGA.Domain.Entities.Notificaciones
{
    public class Notificacion : BaseEntity
    {
        public int UsuarioTransporteId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public bool Leida { get; set; }

        public virtual UsuarioTransporte? Usuario { get; set; }
    }
}
