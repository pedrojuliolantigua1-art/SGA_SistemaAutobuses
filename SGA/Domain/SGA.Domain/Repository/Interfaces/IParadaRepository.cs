using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IParadaRepository : IBaseRepository<Parada, ParadaModel>
    {

        /// <summary>
        /// Trae todas las paradas de una ruta ordenadas por su campo Orden
        /// </summary>
        /// <param name="rutaId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<ParadaModel>> GetByRuta(int rutaId);
    }
}