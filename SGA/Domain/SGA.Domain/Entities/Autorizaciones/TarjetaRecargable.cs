using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Autorizaciones
{
    public class TarjetaRecargable: AutorizacionTransporte
    {
        public string? NumeroTarjeta {  get; set; }
        public decimal SaldoDisponible { get; set; }

    }
}
