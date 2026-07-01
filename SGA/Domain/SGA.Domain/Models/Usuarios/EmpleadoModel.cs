
namespace SGA.Domain.Models.Usuarios
{
    public class EmpleadoModel : UsuarioModel
    {
        public string? CodigoEmpleado { get; set; }
        public string? Departamento { get; set; }
        public string? Cargo { get; set; }
    }
}