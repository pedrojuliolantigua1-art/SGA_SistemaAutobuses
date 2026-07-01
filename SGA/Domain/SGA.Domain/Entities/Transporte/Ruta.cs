using SGA.Domain.Base;

namespace SGA.Domain.Entities.Transporte
{
    public class Ruta : BaseEntity 
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activa { get; set; }
    }
}
