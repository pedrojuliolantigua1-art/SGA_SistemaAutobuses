using SGA.Domain.Entities.Accesos;
using SGA.Domain.Models.Accesos;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IAccesoRepository : IBaseRepository<RegistroUsoTransporte, AccesoModel>
    {
        /// <summary>
        /// Este obtiene todos los registros de acceso de un viaje específico, incluyendo los detalles del usuario y la autorización.
        /// </summary>
        /// <param name="viajeId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<AccesoModel>> GetByViaje(int viajeId);

        /// <summary>
        /// Este obtiene todos los registros de acceso de un usuario específico, incluyendo los detalles del viaje y la autorización.
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<AccesoModel>> GetByUsuario(int usuarioId);

        /// <summary>
        /// Este obtiene todos los registros de acceso dentro de un rango de fechas específico, incluyendo los detalles del usuario, viaje y autorización.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        Task<IReadOnlyList<AccesoModel>> GetByPeriodo(DateTime desde, DateTime hasta);

        //Esto es para la transiccion
        Task<int> RegistrarAbordajeAsync(
            int usuarioId, int viajeId, int autorizacionId,
            int resultadoAcceso, string? motivoRechazo,
            DateTime fechaHora, int validadorId,
            string creadoPor, decimal costoViaje = 60.00m);
    }
}
