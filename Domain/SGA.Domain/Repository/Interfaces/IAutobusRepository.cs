using SGA.Domain.Entities.Transporte;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IAutobusRepository : IBaseRepository<Autobus>
    {
        /// <summary>
        /// trae todos los autobuses disponibles
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<Autobus>> GetDisponibles();
    }
}