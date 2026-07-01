using SGA.Domain.Base;
using SGA.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Usuarios
{
    public abstract class UsuarioTransporte: BaseEntity
    {
        //public string? CodigoInstitucional { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? TipoUsuario { get; set; }
        public string Estado { get; set; } = "Activo";
        public RolUsuario RolSistema { get; set; }
        public string? PasswordHash { get; set; }
    }
}
