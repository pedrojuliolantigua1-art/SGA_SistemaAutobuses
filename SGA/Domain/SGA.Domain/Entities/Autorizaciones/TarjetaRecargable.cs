namespace SGA.Domain.Entities.Autorizaciones
{
    public class TarjetaRecargable : AutorizacionTransporte
    {
        public string? NumeroTarjeta { get; set; }
        public decimal SaldoDisponible { get; set; }

        public virtual ICollection<RecargaTarjeta> Recargas { get; set; } = new List<RecargaTarjeta>();
    }
}
