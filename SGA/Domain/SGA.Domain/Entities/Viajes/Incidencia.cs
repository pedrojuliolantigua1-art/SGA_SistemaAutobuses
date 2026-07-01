using SGA.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Viajes
{
    public class Incidencia : BaseEntity
    {
        public int ViajeId { get; set; }
        public int ConductorId { get; set; }
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
