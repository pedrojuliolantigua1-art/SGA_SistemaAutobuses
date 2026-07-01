using SGA.Domain.Base;

namespace SGA.Domain.Entities.Transporte
{
    public class Autobus : BaseEntity
    {
        public string? Placa {  get; set; }
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; } = "Disponible";
    }
}
