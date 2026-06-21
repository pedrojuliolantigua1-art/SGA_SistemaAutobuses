using SGA.Domain.Entities.Fotos;

namespace SGA.Domain.Repository.Interfaces
{
 
    public interface IFotoAutobusRepository : IBaseRepository<FotoAutobus>
    {
        //Trae la foto activa de un autobus
        Task<FotoAutobus?> GetByAutobusId(int autobusId);

        //<summary>Trae todas las fotos de un autobus
        Task<IReadOnlyList<FotoAutobus>> GetAllByAutobusId(int autobusId);
    }
}