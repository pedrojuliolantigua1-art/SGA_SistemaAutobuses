using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Models.Auditoria;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IAuditoriaRepository : IBaseRepository<RegistroAuditoria, AuditoriaModel>
    {
        /// <summary>
        /// Traer los eventos de auditoria de un periodo
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<AuditoriaModel>> GetbyPeriodo(DateTime desde, DateTime hasta);
        /// <summary>
        /// Traer los eventos generados por un usuario especifico
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<AuditoriaModel>> GetByActor(int usuarioId);
        /// <summary>
        /// Traer eventos de un tipo de acción como tipo validaracceso o de emitir tickets
        /// </summary>
        /// <param name="accion"></param>
        /// <returns></returns>
        Task<IReadOnlyList<AuditoriaModel>> GetbyAccion(string accion);


    }
}
