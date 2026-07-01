
namespace SGA.Domain.Models.Autorizaciones
{
    public class PermisoTransporteModel : AutorizacionModel
    {
        public string? CondicionInstitucional { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}