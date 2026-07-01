using SGA.Application.DTOs.Usuarios;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<Result<IReadOnlyList<UsuarioDto>>> ListarTodosAsync();
        Task<Result<UsuarioDto>> ObtenerPorIdAsync(int id);
        Task<Result<UsuarioDto>> ObtenerPorCorreoAsync(string correo);
        Task<Result<UsuarioDto>> RegistrarAsync(CrearUsuarioDto dto);
        Task<Result<UsuarioDto>> ActualizarAsync(ActualizarUsuarioDto dto);
        Task<Result> DesactivarAsync(int id, string? eliminadoPor);
        Task<Result<bool>> ValidarPasswordAsync(AutenticarDto dto);
    }
}