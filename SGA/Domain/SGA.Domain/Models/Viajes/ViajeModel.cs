using SGA.Domain.Enum;

namespace SGA.Domain.Models.Viajes
{
    public class ViajeModel
    {
        public int Id { get; set; }
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
    }
}