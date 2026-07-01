using SGA.Domain.Entities.Transporte;
using SGA.Domain.Models.Transporte;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IRutaRepository : IBaseRepository<Ruta, RutaModel>
    {
        //Trae solo las rutas activas para mostrar al usuario
        Task<IReadOnlyList<RutaModel>> GetActivas();

        //>Trae las paradas de una ruta especifica ordenadas
        Task<IReadOnlyList<ParadaModel>> GetParadas(int rutaId);

        //Trae los horarios disponibles de una ruta
        Task<IReadOnlyList<HorarioModel>> GetHorarios(int rutaId);
    }
}