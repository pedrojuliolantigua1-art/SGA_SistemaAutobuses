using SGA.Domain.Base;

namespace SGA.Domain.Entities.Transporte
{
    public class HorarioRuta : BaseEntity
    {
        public int RutaId { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraLlegadaEstimada { get; set; }
        public bool Activo { get; set; }

        public virtual Ruta? Ruta { get; set; }
    }
}
