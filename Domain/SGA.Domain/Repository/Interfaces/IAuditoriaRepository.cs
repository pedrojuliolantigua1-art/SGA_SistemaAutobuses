using SGA.Domain.Entities.Auditoria;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IAuditoriaRepository : IBaseRepository<RegistroAuditoria>
    {
        /// <summary>
        /// Traer los eventos de auditoria de un periodo
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<RegistroAuditoria>> Getby_Periodo(DateTime desde, DateTime hasta);
        /// <summary>
        /// Traer los eventos generados por un usuario especifico
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<RegistroAuditoria>> GetBy_Actor(int usuarioId);
        /// <summary>
        /// Traer eventos de un tipo de acción como tipo validaracceso o de emitir tickets
        /// </summary>
        /// <param name="accion"></param>
        /// <returns></returns>
        Task<IReadOnlyList<RegistroAuditoria>> Getby_Accion(string accion);


    }
}
