
namespace SGA.Domain.Entities.Usuarios
{
    public class Empleado : UsuarioTransporte
    {
        public string? CodigoEmpleado { get; set; }
        public string? Departamento { get; set; }
        public string? Cargo { get; set; }
    }
}
