using SGA.Domain.Base;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;

namespace SGA.Domain.Entities.Accesos
{
    public class RegistroUsoTransporte : BaseEntity
    {
        public int UsuarioTransporteId { get; set; }
        public int ViajeId { get; set; }
        public int? AutorizacionTransporteId { get; set; }
        public ResultadoAcceso ResultadoAcceso { get; set; }
        public string? MotivoRechazo { get; set; }
        public DateTime FechaHora { get; set; }
        public int ValidadoPorUsuarioId { get; set; }

        public virtual UsuarioTransporte? Usuario { get; set; }
        public virtual UsuarioTransporte? ValidadoPor { get; set; }
        public virtual Viaje? Viaje { get; set; }
    }
}
