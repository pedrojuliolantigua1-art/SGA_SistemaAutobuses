using SGA.Domain.Entities.Accesos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IAccesoRepository: IBaseRepository<RegistroUsoTransporte>
    {
        /// <summary>
        /// Traer los intentos de acceso a un viaje especifico
        /// </summary>
        /// <param name="viajeId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<RegistroUsoTransporte>> GetBy_Viaje(int viajeId);

        /// <summary>
        /// buscar el historial de intentos de acceso de un usuario a cualquiera de los viajes
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<RegistroUsoTransporte>> GetBy_Usuario(int usuarioId);
        /// <summary>
        /// Esta es para buscar todos los accesos en un periodo para los reportes
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<RegistroUsoTransporte>> GetBy_Periodo(DateTime desde, DateTime hasta);

    }
}
