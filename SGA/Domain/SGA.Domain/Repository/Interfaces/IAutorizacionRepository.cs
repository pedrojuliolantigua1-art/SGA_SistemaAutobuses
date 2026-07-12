using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Models.Autorizaciones;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IAutorizacionRepository : IBaseRepository<AutorizacionTransporte, AutorizacionModel>
    {
        /// <summary>
        /// Aqui se obtiene la autorizacion vigente de un usuario, si no tiene vigente devuelve null
        /// </summary>
        /// <param name="UsuarioId"></param>
        /// <returns></returns>
        Task<AutorizacionModel> GetbyUsuario(int UsuarioId);

        /// <summary>
        /// Este metodo obtiene todas las autorizaciones vigentes, es decir, aquellas que no han expirado y que estan activas
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<AutorizacionModel>> GetVigentes();

        /// <summary>
        /// Este metodo obtiene todas las autorizaciones emitidas en un periodo de tiempo determinado, desde y hasta inclusive
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<AutorizacionModel>> GetbyPeriodo(DateTime desde, DateTime hasta);

        /// <summary>
        /// Busca una tarjeta recargable por su numero, para validar que sea unico al emitir una nueva
        /// </summary>
        Task<TarjetaRecargableModel?> GetByNumeroTarjeta(string numeroTarjeta);

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
