
namespace SGA.Domain.Entities.Usuarios
{
    public class Estudiante: UsuarioTransporte
    {
        public string? Matricula { get; set; }
        public string? Carrera { get; set; }
    }
}
