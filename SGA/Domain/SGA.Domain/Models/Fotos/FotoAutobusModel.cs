

namespace SGA.Domain.Models.Fotos
{
    public class FotoAutobusModel
    {
        public int Id { get; set; }
        public int AutobusId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string UrlPublica { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string SubidoPor { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; }
    }
}