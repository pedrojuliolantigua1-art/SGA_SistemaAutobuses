using SGA.Domain.Entities.Fotos;
using SGA.Domain.Models.Fotos;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IFotoIncidenciaRepository : IBaseRepository<FotoIncidencia, FotoIncidenciaModel>
    {
        /// <summary>
        /// Obtiene una lista de fotos asociadas a una incidencia específica.
        /// </summary>
        /// <param name="incidenciaId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<FotoIncidenciaModel>> GetByIncidenciaId(int incidenciaId);
        /// <summary>
        /// obtiene una lista de fotos asociadas a un viaje específico.
        /// </summary>
        /// <param name="viajeId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<FotoIncidenciaModel>> GetByViajeId(int viajeId);
    }
}