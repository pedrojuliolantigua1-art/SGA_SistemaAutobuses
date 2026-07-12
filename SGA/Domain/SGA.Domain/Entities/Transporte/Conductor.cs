using SGA.Domain.Entities.Usuarios;

namespace SGA.Domain.Entities.Transporte
{
    public class Conductor: UsuarioTransporte
    {
        public string? NumeroLicencia { get; set; }
        public DateTime? FechaVencimientoLicencia { get; set; }
        public bool Disponible { get; set; }
    }
}
