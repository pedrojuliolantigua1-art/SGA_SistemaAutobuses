using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Usuarios;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<Result<IReadOnlyList<UsuarioResumenDto>>> ListarTodosAsync();
        Task<Result<UsuarioResumenDto>> ObtenerPorIdAsync(int id);
        Task<Result<UsuarioResumenDto>> ObtenerPorCorreoAsync(string correo);

        Task<Result<EstudianteDto>> ObtenerEstudiantePorMatriculaAsync(string matricula);

        Task<Result<ConductorDto>> ObtenerConductorPorLicenciaAsync(string numeroLicencia);
        Task<Result<bool>> ValidarPasswordAsync(AutenticarDto dto);

        Task<Result<EstudianteDto>> RegistrarEstudianteAsync(CrearEstudianteDto dto);
        Task<Result<EstudianteDto>> ActualizarEstudianteAsync(int id, ActualizarEstudianteDto dto);

        Task<Result<EmpleadoDocenteDto>> RegistrarEmpleadoDocenteAsync(CrearEmpleadoDocenteDto dto);
        Task<Result<EmpleadoDocenteDto>> ActualizarEmpleadoDocenteAsync(int id, ActualizarEmpleadoDocenteDto dto);

        Task<Result<EmpleadoAdministrativoDto>> RegistrarEmpleadoAdministrativoAsync(CrearEmpleadoAdministrativoDto dto);
        Task<Result<EmpleadoAdministrativoDto>> ActualizarEmpleadoAdministrativoAsync(int id, ActualizarEmpleadoAdministrativoDto dto);

        Task<Result<ConductorDto>> RegistrarConductorAsync(CrearConductorDto dto);
        Task<Result<ConductorDto>> ActualizarConductorAsync(int id, ActualizarConductorDto dto);
        Task<Result<ConductorDto>> CambiarDisponibilidadAsync(int id, CambiarDisponibilidadConductorDto dto);

        Task<Result> EliminarAsync(int id, EliminarDto dto);
    }
}
