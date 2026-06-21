using SGA.Domain.Entities.Fotos;

namespace SGA.Domain.Repository.Interfaces
{

    public interface IFotoIncidenciaRepository : IBaseRepository<FotoIncidencia>
    {
        // Este trae todas las fotos de una incidencia especifica
        Task<IReadOnlyList<FotoIncidencia>> GetByIncidenciaId(int incidenciaId);

        //>trae todas las fotos de las incidencias de un viaje
        Task<IReadOnlyList<FotoIncidencia>> GetByViajeId(int viajeId);
    }
}