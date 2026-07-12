using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules
{
    public static class UsuarioBaseRules
    {
        //para verificar el correo del itla
        public static Result ValidarCorreoInstitucional(string? correo)
        {
            var formatoBasico = ValidationGeneral.FormatoValido(correo, "correo",
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$", "correo electronico valido");

            if (formatoBasico.EsFallo)
            {
                return formatoBasico;
            }

            if (correo!.Trim().EndsWith("@itla.edu.do", StringComparison.OrdinalIgnoreCase))
            {
                return Result.Ok();
            }
            else
            {
                return Result.Fallo(
                    DomainErrors.General.FormatoInvalido("correo", "correo institucional @itla.edu.do"));
            }
        }

        public static Result ValidarDatosBase(UsuarioTransporte? usuario)
        {
            if (usuario is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("usuario"));
            }

            return ValidationGeneral.Combinar(
                //Lo puse asi por que no existe nombre o apellido de una sola letra en ningun pais caracteres
                ValidationGeneral.RequeridoConLongitud(usuario.Nombre, "nombre", min: 2, max: 80),
                ValidationGeneral.RequeridoConLongitud(usuario.Apellido, "apellido", min: 2, max: 80),
                ValidarCorreoInstitucional(usuario.Correo));
        }

        public static bool EstaActivo(string? estado)
        {
            if (string.Equals(estado, "Activo", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}