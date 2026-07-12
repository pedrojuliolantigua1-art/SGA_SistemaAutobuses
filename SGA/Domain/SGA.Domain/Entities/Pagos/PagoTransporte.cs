using SGA.Domain.Base;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Enum;

namespace SGA.Domain.Entities.Pagos
{
    public class PagoTransporte : BaseEntity
    {
        public int UsuarioTransporteId { get; set; }
        public int AutorizacionTransporteId { get; set; }
        public decimal Monto { get; set; }
        public string? TipoPago { get; set; }
        public EstadoPago Estado { get; set; } = EstadoPago.Registrado;
        public string? NumeroComprobante { get; set; }
        public DateTime FechaHora { get; set; }
        public int RegistradoPorUsuarioId { get; set; }

        public virtual UsuarioTransporte? Usuario { get; set; }
        public virtual UsuarioTransporte? RegistradoPor { get; set; }
    }
}
