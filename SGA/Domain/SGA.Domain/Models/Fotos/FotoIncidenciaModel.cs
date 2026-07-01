

namespace SGA.Domain.Models.Fotos
{
    public class FotoIncidenciaModel
    {
        public int Id { get; set; }
        public int IncidenciaId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string UrlPublica { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string SubidoPor { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; }
    }
}