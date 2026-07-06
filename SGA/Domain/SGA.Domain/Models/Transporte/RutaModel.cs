namespace SGA.Domain.Models.Transporte
{
    public class RutaModel
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activa { get; set; }

        public List<ParadaModel> Paradas { get; set; } = new();
        public List<HorarioModel> Horarios { get; set; } = new();
    }
}
