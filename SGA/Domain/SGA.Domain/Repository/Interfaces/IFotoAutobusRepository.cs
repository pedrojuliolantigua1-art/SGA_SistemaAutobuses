using SGA.Domain.Entities.Fotos;
using SGA.Domain.Models.Fotos;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IFotoAutobusRepository : IBaseRepository<FotoAutobus, FotoAutobusModel>
    {
        /// <summary>
        /// obtiene la foto del autobus por su id
        /// </summary>
        /// <param name="autobusId"></param>
        /// <returns></returns>
        Task<FotoAutobusModel?> GetByAutobusId(int autobusId);

        /// <summary>
        /// obtiene todas las fotos del autobus por su id
        /// </summary>
        /// <param name="autobusId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<FotoAutobusModel>> GetAllByAutobusId(int autobusId);
    }
}