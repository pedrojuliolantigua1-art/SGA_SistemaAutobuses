
namespace SGA.Domain.Models.Usuarios
{
    public class ConductorModel : UsuarioModel
    {
        public string? NumeroLicencia { get; set; }
        public DateTime? FechaVencimientoLicencia { get; set; }
        public bool Disponible { get; set; }
    }
}