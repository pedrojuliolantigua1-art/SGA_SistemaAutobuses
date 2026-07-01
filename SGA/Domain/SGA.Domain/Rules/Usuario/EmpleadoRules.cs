using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules
{
    public static class EmpleadoRules
    {
        // puse este de ejemplo Trab(trabajador tal para enumerarlo) y un codigo hasta que averigue como son en realidad
        private const string PatronCodigoEmpleado = @"^Trab-\d{4,8}$";

        public static Result ValidarCodigoEmpleado(string? codigoEmpleado) =>
            ValidationGeneral.FormatoValido (codigoEmpleado, "codigo de empleado", PatronCodigoEmpleado,
            "Trab- luego 4 o 8 numeros (ej: Trab-034532)");

        public static Result Validar(Empleado? empleado)
        {
            if (empleado is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("empleado"));
            }

            return ValidationGeneral.Combinar(
                UsuarioBaseRules.ValidarDatosBase(empleado),
                ValidarCodigoEmpleado(empleado.CodigoEmpleado));
        }
    }
}