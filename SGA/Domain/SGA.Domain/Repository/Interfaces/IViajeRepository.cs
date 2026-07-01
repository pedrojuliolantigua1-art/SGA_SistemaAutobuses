using SGA.Domain.Entities.Viajes;
using SGA.Domain.Models.Viajes;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IViajeRepository : IBaseRepository<Viaje, ViajeModel>
    {
        Task<IReadOnlyList<ViajeModel>> GetbyFecha(DateTime fecha);
        Task<IReadOnlyList<ViajeModel>> GetbyConductor(int conductorId);
        Task<IReadOnlyList<ViajeModel>> GetbyAutobusActivo(int autobusId);
        Task<IReadOnlyList<ViajeModel>> GetbyPeriodo(DateTime desde, DateTime hasta);
        Task AddIncidencia(Incidencia incidencia);
        Task<IReadOnlyList<IncidenciaModel>> GetIncidenciasbyPeriodo(DateTime desde, DateTime hasta);

        // Cancela el viaje, registra incidencia y registra auditoria en una sola transaccion.
        // Retorna el Id luego
        Task<int> CancelarViajeAsync(
            int viajeId, int conductorId, string motivo,
            DateTime fechaHora, int canceladoPorId, string creadoPor);
    }
}
