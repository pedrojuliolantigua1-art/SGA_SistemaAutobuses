
namespace SGA.Domain.Models.Transporte
{
    public class AutobusModel
    {
        public int Id { get; set; }
        public string? Placa { get; set; }
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; } = "Disponible";
    }
}