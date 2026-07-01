
namespace SGA.Domain.Models.Usuarios
{
    public class ConductorModel : UsuarioModel
    {
        public string? NumeroLicencia { get; set; }
        public bool Disponible { get; set; }
    }
}