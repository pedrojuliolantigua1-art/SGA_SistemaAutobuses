using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IHorarioRutaRepository : IBaseRepository<HorarioRuta, HorarioModel>
    {
        /// <summary>
        /// Trae los horarios activos de una ruta especifica
        /// </summary>
        /// <param name="rutaId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<HorarioModel>> GetByRuta(int rutaId);
        /// <summary>
        /// Muestra todos los horarios eliminados
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<HorarioModel>> GetEliminados();
    }
}