using SGA.Domain.Models.Fotos;

namespace SGA.Domain.Models.Viajes
{
    public class IncidenciaModel
    {
        public int Id { get; set; }
        public int ViajeId { get; set; }
        public int ConductorId { get; set; }
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaHora { get; set; }

        public string? ConductorNombre { get; set; }
        public List<FotoIncidenciaModel> Fotos { get; set; } = new();
    }
}
