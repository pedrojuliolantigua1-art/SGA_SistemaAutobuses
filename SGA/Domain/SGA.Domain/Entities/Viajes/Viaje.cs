using SGA.Domain.Base;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Enum;

namespace SGA.Domain.Entities.Viajes
{
    public class Viaje : BaseEntity
    {
        public int RutaId { get; set; }
        public int HorarioRutaId { get; set; }
        public int AutobusId { get; set; }
        public int ConductorId { get; set; }
        public DateTime Fecha { get; set; }
        public EstadoViaje Estado { get; set; }
        public DateTime? HoraInicioReal { get; set; }
        public DateTime? HoraFinReal { get; set; }
        public int CupoActual { get; set; }
        public int CapacidadMaxima { get; set; }

        public virtual Ruta? Ruta { get; set; }
        public virtual HorarioRuta? HorarioRuta { get; set; }
        public virtual Autobus? Autobus { get; set; }
        public virtual Conductor? Conductor { get; set; }
        public virtual ICollection<Incidencia> Incidencias { get; set; } = new List<Incidencia>();
    }
}
