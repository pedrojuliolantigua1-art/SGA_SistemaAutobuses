using SGA.Domain.Base;

namespace SGA.Domain.Entities.Fotos
{

    public class FotoAutobus : BaseEntity
    {
        public int AutobusId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string UrlPublica { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty; //este Id en Cloudinary es para poder eliminarla
        public string SubidoPor { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; }
    }
}
