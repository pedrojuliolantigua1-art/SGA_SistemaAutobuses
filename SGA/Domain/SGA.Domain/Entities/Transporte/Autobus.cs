using SGA.Domain.Base;
using SGA.Domain.Entities.Fotos;

namespace SGA.Domain.Entities.Transporte
{
    public class Autobus : BaseEntity
    {
        public string? Placa { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; } = "Disponible";

        public virtual ICollection<FotoAutobus> Fotos { get; set; } = new List<FotoAutobus>();
    }
}
