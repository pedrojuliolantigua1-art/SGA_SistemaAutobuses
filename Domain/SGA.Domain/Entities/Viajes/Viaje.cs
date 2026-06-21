using SGA.Domain.Base;
using SGA.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Viajes
{
    public class Viaje : BaseEntity
    {
        public int RutaId { get; set; }
        public int HorarioRutaId { get; set; }
        public int AutobusId { get; set; }
        public int ConductorId { get; set; }
        public DateTime Fecha {  get; set; }
        public EstadoViaje Estado { get; set; }
        public DateTime? HoraInicioReal {  get; set; }
        public DateTime? HoraFinReal { get; set; }
        public int CupoActual { get; set; }
        public int CapacidadMaxima {  get; set; }

    }
}
