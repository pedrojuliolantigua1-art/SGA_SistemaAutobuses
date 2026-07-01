
namespace SGA.Domain.Models.Usuarios
{
    public class EstudianteModel : UsuarioModel
    {
        public string? Matricula { get; set; }
        public string? Carrera { get; set; }
    }
}