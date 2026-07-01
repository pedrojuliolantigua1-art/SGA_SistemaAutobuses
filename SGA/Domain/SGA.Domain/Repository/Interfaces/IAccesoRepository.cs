using SGA.Domain.Entities.Accesos;
using SGA.Domain.Models.Accesos;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IAccesoRepository : IBaseRepository<RegistroUsoTransporte, AccesoModel>
    {
        Task<IReadOnlyList<AccesoModel>> GetByViaje(int viajeId);
        Task<IReadOnlyList<AccesoModel>> GetByUsuario(int usuarioId);
        Task<IReadOnlyList<AccesoModel>> GetByPeriodo(DateTime desde, DateTime hasta);

        //Esto es para la transiccion
        Task<int> RegistrarAbordajeAsync(
            int usuarioId, int viajeId, int autorizacionId,
            int resultadoAcceso, string? motivoRechazo,
            DateTime fechaHora, int validadorId,
            string creadoPor, decimal costoViaje = 60.00m);
    }
}
