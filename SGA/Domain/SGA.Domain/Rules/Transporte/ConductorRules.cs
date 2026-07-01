using SGA.Domain.Entities.Transporte;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules
{
    public static class ConductorRules
    {
        // la licencia aqui son como la cedula 111 caracteres solamente 
        private const string PatronNumeroLicencia = @"^\d{11}$";

        public static Result ValidarNumeroLicencia(string? numeroLicencia) =>
            ValidationGeneral.FormatoValido(numeroLicencia, "numero de licencia", PatronNumeroLicencia, "11 numeros solamente");

        public static Result Validar(Conductor? conductor)
        {
            if (conductor is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("conductor"));
            }

            return ValidationGeneral.Combinar(UsuarioBaseRules.ValidarDatosBase(conductor),
                ValidarNumeroLicencia(conductor.NumeroLicencia));
        }

        public static Result ValidarParaAsignacion(Conductor? conductor)
        {
            var validacion = Validar(conductor);
            if (validacion.EsFallo)
            {
                return validacion;
            }

            var idValido = ValidationGeneral.IdValido(conductor!.Id, "conductor");

            if (idValido.EsFallo)
            {
                return idValido;
            }

            if (conductor.Disponible && UsuarioBaseRules.EstaActivo(conductor.Estado))
            {
                return Result.Ok();
            }

            return Result.Fallo(DomainErrors.CatalogoTransporte.ConductorNoDisponible);
        }
    }
}