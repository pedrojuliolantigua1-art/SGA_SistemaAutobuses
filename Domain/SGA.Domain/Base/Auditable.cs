
namespace SGA.Domain.Base
{
    public abstract class Auditable
    {
        public DateTime FechaCreacion { get; set; }
        public string? CreadoPor { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaEliminacion { get; set; }
        public string? EliminadoPor { get; set; }
    }
}
