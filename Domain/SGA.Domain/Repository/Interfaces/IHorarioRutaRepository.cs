using SGA.Domain.Entities.Transporte;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IHorarioRutaRepository : IBaseRepository<HorarioRuta>
    {
        /// <summary>
        /// Trae los horarios activos de una ruta especifica
        /// </summary>
        /// <param name="rutaId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<HorarioRuta>> GetByRuta(int rutaId);
    }
}