using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Repository.Interfaces
{
    public interface IUsuarioRepository : IBaseRepository<UsuarioTransporte>
    {
        /// <summary>
        /// Busco el usuario por el correo
        /// </summary>
        /// <param name="correo"></param>
        /// <returns></returns>
        Task<UsuarioTransporte?> Getby_Correo(string correo);
        /// <summary>
        /// Saco todos los usuarios de un rol especifico
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        Task<UsuarioTransporte> Getby_Rol(RolUsuario rol);
        /// <summary>
        /// Ver si la contraseña coincide con el hash que se almaceno
        /// </summary>
        /// <param name="correo"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        Task<bool> Validar_Password (string correo, string passwordHash); 
        

    }
}
