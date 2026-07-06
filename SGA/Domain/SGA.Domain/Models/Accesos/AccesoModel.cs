using SGA.Domain.Enum;

namespace SGA.Domain.Models.Accesos
{
    public class AccesoModel
    {
        public int Id { get; set; }
        public int UsuarioTransporteId { get; set; }
        public int ViajeId { get; set; }
        public int? AutorizacionTransporteId { get; set; }
        public ResultadoAcceso ResultadoAcceso { get; set; }
        public string? MotivoRechazo { get; set; }
        public DateTime FechaHora { get; set; }
        public int ValidadoPorUsuarioId { get; set; }

        public string? UsuarioNombre { get; set; }
        public string? ValidadorNombre { get; set; }
    }
}
