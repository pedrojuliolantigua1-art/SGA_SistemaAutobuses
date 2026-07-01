using SGA.Domain.Entities.Fotos;
using SGA.Domain.Models.Fotos;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IFotoAutobusRepository : IBaseRepository<FotoAutobus, FotoAutobusModel>
    {
        Task<FotoAutobusModel?> GetByAutobusId(int autobusId);
        Task<IReadOnlyList<FotoAutobusModel>> GetAllByAutobusId(int autobusId);
    }
}