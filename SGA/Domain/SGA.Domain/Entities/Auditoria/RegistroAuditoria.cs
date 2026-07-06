using SGA.Domain.Base;
using SGA.Domain.Entities.Usuarios;

namespace SGA.Domain.Entities.Auditoria
{
    public class RegistroAuditoria : BaseEntity
    {
        public int UsuarioTransporteId { get; set; }
        public string? Accion { get; set; }
        public string? EntidadAfectada { get; set; }
        public string? EntidadId { get; set; }
        public string? Detalle { get; set; }
        public DateTime FechaHora { get; set; }

        public virtual UsuarioTransporte? Usuario { get; set; }
    }
}
