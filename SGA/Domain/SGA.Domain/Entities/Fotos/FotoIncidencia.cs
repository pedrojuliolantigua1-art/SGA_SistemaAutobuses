using SGA.Domain.Base;
using SGA.Domain.Entities.Viajes;

namespace SGA.Domain.Entities.Fotos
{
    public class FotoIncidencia : BaseEntity
    {
        public int IncidenciaId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string UrlPublica { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string SubidoPor { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; }

        public virtual Incidencia? Incidencia { get; set; }
    }
}
