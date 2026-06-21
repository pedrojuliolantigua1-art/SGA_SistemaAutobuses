using SGA.Domain.Entities.Transporte;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IParadaRepository : IBaseRepository<Parada>
    {
        //<summary>Trae todas las paradas de una ruta ordenadas por su campo Orden
        Task<IReadOnlyList<Parada>> GetByRuta(int rutaId);
    }
}