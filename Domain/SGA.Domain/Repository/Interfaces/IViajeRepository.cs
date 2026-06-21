using SGA.Domain.Entities.Viajes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IViajeRepository : IBaseRepository<Viaje>
    {
        /// <summary>
        /// Obtengo por fecha los viajes que hay
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Viaje>> Getby_Fecha(DateTime fecha);
        /// <summary>
        /// Para ver los viajes respecto al conductor o sea que viaje tiene asignado tal conductor
        /// </summary>
        /// <param name="conductorId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Viaje>> Getby_Conductor(int conductorId);
        /// <summary>
        /// Para verificar si un autobus esta ya en un viaje 
        /// </summary>
        /// <param name="autobusId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Viaje>> Getby_AutobusActivo(int autobusId);
        /// <summary>
        /// Para poder ver los viajes dentro de un periodo de tiempo
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Viaje>> Getby_Periodo(DateTime desde, DateTime hasta);
        /// <summary>
        ///  Para agregar una incidencia de un viaje
        /// </summary>
        /// <param name="incidencia"></param>
        /// <returns></returns>
        Task AddIncidencia(Incidencia incidencia);
        /// <summary>
        /// Para buscar las incidencias por periodo de tiempo
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Incidencia>> GetIncidencias_byPeriodo(DateTime desde, DateTime hasta);


    }
}
