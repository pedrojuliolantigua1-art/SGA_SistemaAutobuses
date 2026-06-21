using SGA.Domain.Entities.Pagos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IPagoRepository : IBaseRepository<PagoTransporte>
    {
        /// <summary>
        /// trae todos los pagos registrados para un usuario
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<PagoTransporte>> GetBy_Usuario(int usuarioId);
        /// <summary>
        /// Verificar si hay una autorizacion asociada
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        Task<PagoTransporte?> Get_PagoSinAutorizacion(int usuarioId);


    }
}
