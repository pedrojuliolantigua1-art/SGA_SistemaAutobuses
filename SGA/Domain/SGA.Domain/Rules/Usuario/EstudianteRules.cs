using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules
{
    public static class EstudianteRules
    {
        // el patron de la matricula del itla que es solamente el año y 4 digitos mas y ya que se suman
        private const string PatronMatricula = @"^\d{4}-\d{4}$";

        public static Result ValidarMatricula(string? matricula) =>
            ValidationGeneral.FormatoValido(matricula, "matricula", PatronMatricula, "Year-NNNN (un ejemplo: 2025-2022)");

        public static Result Validar(Estudiante? estudiante)
        {
            if (estudiante is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("estudiante"));
            }

            return ValidationGeneral.Combinar(UsuarioBaseRules.ValidarDatosBase(estudiante),
            ValidarMatricula(estudiante.Matricula));
        }
    }
}