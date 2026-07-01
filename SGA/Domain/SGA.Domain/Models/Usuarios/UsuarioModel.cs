using SGA.Domain.Enum;

namespace SGA.Domain.Models.Usuarios
{
    public abstract class UsuarioModel
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string Estado { get; set; } = "Activo";
        public RolUsuario RolSistema { get; set; }
    }
}
