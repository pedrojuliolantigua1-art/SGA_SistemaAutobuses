using SGA.Domain.Base;
using SGA.Domain.Entities.Transporte;

namespace SGA.Domain.Entities.Fotos
{
    public class FotoAutobus : BaseEntity
    {
        public int AutobusId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string UrlPublica { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string SubidoPor { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; }

        public virtual Autobus? Autobus { get; set; }
    }
}
