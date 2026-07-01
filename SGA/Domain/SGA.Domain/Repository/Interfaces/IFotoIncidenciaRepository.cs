using SGA.Domain.Entities.Fotos;
using SGA.Domain.Models.Fotos;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IFotoIncidenciaRepository : IBaseRepository<FotoIncidencia, FotoIncidenciaModel>
    {
        Task<IReadOnlyList<FotoIncidenciaModel>> GetByIncidenciaId(int incidenciaId);
        Task<IReadOnlyList<FotoIncidenciaModel>> GetByViajeId(int viajeId);
    }
}