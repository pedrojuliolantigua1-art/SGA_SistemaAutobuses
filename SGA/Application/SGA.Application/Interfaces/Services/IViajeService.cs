using SGA.Application.DTOs.Viajes;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IViajeService
    {
        // lista viajes por fecha
        Task<Result<IReadOnlyList<ViajeDto>>> ListarPorFechaAsync(DateTime fecha);

        // lista viajes por conductor
        Task<Result<IReadOnlyList<ViajeDto>>> ListarPorConductorAsync(int conductorId);

        // obtiene un viaje por id
        Task<Result<ViajeDto>> ObtenerPorIdAsync(int viajeId);

        // lista viajes en curso
        Task<Result<IReadOnlyList<ViajeDto>>> ListarActivosAsync();

        // lista viajes programados
        Task<Result<IReadOnlyList<ViajeDto>>> ListarProgramadosAsync();

        // lista viajes por ruta
        Task<Result<IReadOnlyList<ViajeDto>>> ListarPorRutaAsync(int rutaId);

        // lista viajes por autobus (historial completo)
        Task<Result<IReadOnlyList<ViajeDto>>> ListarPorAutobusAsync(int autobusId);

        // programa un viaje
        Task<Result<ViajeDto>> ProgramarAsync(ProgramarViajeDto dto);

        // inicia un viaje
        Task<Result<ViajeDto>> IniciarAsync(EjecutarViajeDto dto);

        // finaliza un viaje
        Task<Result<ViajeDto>> FinalizarAsync(EjecutarViajeDto dto);

        // cancela un viaje
        Task<Result<ViajeDto>> CancelarAsync(CancelarViajeDto dto);

        // reporta una incidencia del viaje
        Task<Result<IncidenciaDto>> ReportarIncidenciaAsync(ReportarIncidenciaDto dto);
    }
}
