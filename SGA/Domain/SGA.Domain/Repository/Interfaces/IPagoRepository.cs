using SGA.Domain.Entities.Pagos;
using SGA.Domain.Models.Pagos;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IPagoRepository : IBaseRepository<PagoTransporte, PagoModel>
    {
        /// <summary>
        /// trae todos los pagos registrados para un usuario
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<PagoModel>> GetByUsuario(int usuarioId);
        /// <summary>
        /// Verificar si hay una autorizacion asociada
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        Task<PagoModel?> GetPagoSinAutorizacion(int usuarioId);


    }
}
