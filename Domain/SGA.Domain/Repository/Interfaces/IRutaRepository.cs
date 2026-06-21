using SGA.Domain.Entities.Transporte;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IRutaRepository : IBaseRepository<Ruta>
    {
        //Trae solo las rutas activas para mostrar al usuario
        Task<IReadOnlyList<Ruta>> GetActivas();

        //>Trae las paradas de una ruta especifica ordenadas
        Task<IReadOnlyList<Parada>> GetParadas(int rutaId);

        //Trae los horarios disponibles de una ruta
        Task<IReadOnlyList<HorarioRuta>> GetHorarios(int rutaId);
    }
}