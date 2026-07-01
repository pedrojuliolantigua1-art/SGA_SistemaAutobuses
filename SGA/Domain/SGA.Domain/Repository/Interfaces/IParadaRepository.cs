using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IParadaRepository : IBaseRepository<Parada, ParadaModel>
    {
        //<summary>Trae todas las paradas de una ruta ordenadas por su campo Orden
        Task<IReadOnlyList<ParadaModel>> GetByRuta(int rutaId);
    }
}