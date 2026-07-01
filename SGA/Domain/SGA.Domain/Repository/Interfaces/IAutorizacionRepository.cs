using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Models.Autorizaciones;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IAutorizacionRepository : IBaseRepository<AutorizacionTransporte, AutorizacionModel>
    {
        Task<AutorizacionModel> GetbyUsuario(int UsuarioId);
        Task<IReadOnlyList<AutorizacionModel>> GetVigentes();
        Task<IReadOnlyList<AutorizacionModel>> GetbyPeriodo(DateTime desde, DateTime hasta);

        // la transaccion Registra el pago y crea la autorizacion y
        // registra auditoria en una sola transaccion.
        Task<(int PagoId, int AutorizacionId)> EmitirAutorizacionAsync(
            int usuarioId, decimal monto, string tipoPago,
            string numeroComprobante, DateTime fechaHora, int registradoPorId,
            string tipoAutorizacion, DateTime fechaEmision,
            DateTime? fechaInicio, DateTime? fechaFin,
            string? numeroTarjeta, decimal? saldoInicial,
            string? condicionInstitucional, DateTime? fechaVencimiento,
            string creadoPor);
    }
}
