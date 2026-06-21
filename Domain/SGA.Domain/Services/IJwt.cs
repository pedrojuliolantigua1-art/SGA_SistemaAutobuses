using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Enum;

namespace SGA.Domain.Services
{
    public interface IJwt
    {
        /// <summary>
        /// Genera el token con los claims de usuarioid, nombre, el rol tambbien
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        string GenerarToken(UsuarioTransporte usuario);
        /// <summary>
        /// Valida que el token no haya vencido
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool ValidarToken(string token);

        /// <summary>
        /// trae el id del usuario desde el token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        int ObtenerUsuarioId(string token);
        /// <summary>
        /// trae el rol del usuario desde el token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        RolUsuario ObtenerRol(string token);

    }
}
