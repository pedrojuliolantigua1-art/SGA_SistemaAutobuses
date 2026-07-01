
namespace SGA.Domain.Models.Transporte
{
    public class ParadaModel
    {
        public int Id { get; set; }
        public int RutaId { get; set; }
        public string? Nombre { get; set; }
        public string? Referencia { get; set; }
        public int Orden { get; set; }
    }
}