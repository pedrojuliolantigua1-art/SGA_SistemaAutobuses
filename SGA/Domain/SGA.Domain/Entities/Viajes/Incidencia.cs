using SGA.Domain.Base;
using SGA.Domain.Entities.Fotos;
using SGA.Domain.Entities.Transporte;

namespace SGA.Domain.Entities.Viajes
{
    public class Incidencia : BaseEntity
    {
        public int ViajeId { get; set; }
        public int ConductorId { get; set; }
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaHora { get; set; }

        public virtual Viaje? Viaje { get; set; }
        public virtual Conductor? Conductor { get; set; }
        public virtual ICollection<FotoIncidencia> Fotos { get; set; } = new List<FotoIncidencia>();
    }
}
