namespace SGA.Domain.Models.Autorizaciones
{
    public class TarjetaRecargableModel : AutorizacionModel
    {
        public string? NumeroTarjeta { get; set; }
        public decimal SaldoDisponible { get; set; }

        public List<RecargaTarjetaModel> Recargas { get; set; } = new();
    }
}
