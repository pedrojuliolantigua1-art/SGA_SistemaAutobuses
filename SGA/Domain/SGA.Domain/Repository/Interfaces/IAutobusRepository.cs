using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IAutobusRepository : IBaseRepository<Autobus, AutobusModel>
    {
        /// <summary>
        /// trae todos los autobuses disponibles
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<AutobusModel>> GetDisponibles();

        /// <summary>
        /// busca un autobus por su placa
        /// </summary>
        Task<AutobusModel?> GetByPlaca(string placa);
    }
}