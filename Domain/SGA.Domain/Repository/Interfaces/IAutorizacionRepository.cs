using SGA.Domain.Entities.Autorizaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IAutorizacionRepository : IBaseRepository<AutorizacionTransporte>
    {
        /// <summary>
        /// Para obtener la autorizacion que tiene ese estudiante activa
        /// </summary>
        /// <param name="UsuarioId"></param>
        /// <returns></returns>
        Task<AutorizacionTransporte> Getby_Usuario (int UsuarioId);
        /// <summary>
        /// Trae todas las autorizaciones que estan vigentes
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<AutorizacionTransporte>> Get_Vigentes();
        /// <summary>
        /// Traerla por periodo para tenerla por fecha para el reporte
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<AutorizacionTransporte>> Geby_yPeriodo(DateTime desde, DateTime hasta);

    }
}
