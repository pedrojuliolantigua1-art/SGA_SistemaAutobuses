using SGA.Domain.Base;

namespace SGA.Domain.Entities.Transporte
{
    public class Parada : BaseEntity
    {
        public int RutaId { get; set; }
        public string? Nombre { get; set; }
        public string? Referencia { get; set; }
        public int Orden { get; set; }

        public virtual Ruta? Ruta { get; set; }
    }
}
