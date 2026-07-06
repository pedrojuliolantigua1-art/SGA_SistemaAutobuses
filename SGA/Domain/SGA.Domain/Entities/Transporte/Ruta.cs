using SGA.Domain.Base;

namespace SGA.Domain.Entities.Transporte
{
    public class Ruta : BaseEntity
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activa { get; set; }

        public virtual ICollection<Parada> Paradas { get; set; } = new List<Parada>();
        public virtual ICollection<HorarioRuta> Horarios { get; set; } = new List<HorarioRuta>();
    }
}
