
namespace SGA.Domain.Models.Transporte
{
    public class HorarioModel
    {
        public int Id { get; set; }
        public int RutaId { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraLlegadaEstimada { get; set; }
        public bool Activo { get; set; }
    }
}