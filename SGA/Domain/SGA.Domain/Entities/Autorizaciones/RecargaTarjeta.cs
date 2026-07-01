using SGA.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Autorizaciones
{
    public class RecargaTarjeta : BaseEntity
    {
        public int TarjetaRecargableId { get; set; }
        public int PagoTransporteId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
