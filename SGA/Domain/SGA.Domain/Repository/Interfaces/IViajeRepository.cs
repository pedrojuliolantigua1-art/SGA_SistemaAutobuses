using SGA.Domain.Entities.Viajes;
using SGA.Domain.Models.Viajes;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IViajeRepository : IBaseRepository<Viaje, ViajeModel>
    {
        /// <summary>
        /// obtiene los viajes por fecha, si no hay viajes retorna una lista vacia
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        Task<IReadOnlyList<ViajeModel>> GetbyFecha(DateTime fecha);

        /// <summary>
        /// obtiene los viajes por conductor, si no hay viajes retorna una lista vacia
        /// </summary>
        /// <param name="conductorId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<ViajeModel>> GetbyConductor(int conductorId);

        /// <summary>
        /// obtiene los viajes por autobus activo, si no hay viajes retorna una lista vacia
        /// </summary>
        /// <param name="autobusId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<ViajeModel>> GetbyAutobusActivo(int autobusId);

        /// <summary>
        /// obtiene los viajes por periodo, si no hay viajes retorna una lista vacia
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<ViajeModel>> GetbyPeriodo(DateTime desde, DateTime hasta);

        /// <summary>
        /// anade una incidencia al viaje, si no hay incidencias retorna una lista vacia
        /// </summary>
        /// <param name="incidencia"></param>
        /// <returns></returns>
        Task AddIncidencia(Incidencia incidencia);

        /// <summary>
        /// obtiene las incidencias por periodo, si no hay incidencias retorna una lista vacia
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<IncidenciaModel>> GetIncidenciasbyPeriodo(DateTime desde, DateTime hasta);

        // Cancela el viaje, registra incidencia y registra auditoria en una sola transaccion.
        // Retorna el Id luego
        Task<int> CancelarViajeAsync(
            int viajeId, int conductorId, string motivo,
            DateTime fechaHora, int canceladoPorId, string creadoPor);
    }
}
