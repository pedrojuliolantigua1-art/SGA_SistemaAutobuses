using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Autorizaciones
{
    public class PermisoTransporte : AutorizacionTransporte
    {
        public string? CondicionInstitucional {  get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}
