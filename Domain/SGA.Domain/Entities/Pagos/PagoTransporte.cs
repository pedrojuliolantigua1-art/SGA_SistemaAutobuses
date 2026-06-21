using SGA.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Pagos
{
    public class PagoTransporte : BaseEntity
    {
        public int UsuarioTransporteId { get; set; }
        public int AutorizacionTransporteId { get; set; }
        public decimal Monto { get; set; }
        public string? TipoPago { get; set; }
        public string? Estado {  get; set; }
        public string? NumeroComprobante { get; set; }

        public DateTime FechaHora { get; set; }
        public int RegistradoPorUsuarioId { get; set; }

    }
}
