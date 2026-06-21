
namespace SGA.Domain.Entities.Usuarios
{
    public class EmpleadoDocente : Empleado
    {
        public string? Especialidad { get; set; }
        public string? TipoContrato { get; set; }
    }
}
