using SGA.Domain.Entities.Transporte;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules
{
    public static class AutobusRules
    {
        // la placa aqui en rd siempre son asi segun investigue son 1 o 2 letras seguidas de 6 numeros 
        private const string PatronPlaca = @"^[A-Z]{1,2}\d{6}$";

        public static Result ValidarPlaca(string? placa) =>
            ValidationGeneral.FormatoValido(placa, "placa", PatronPlaca, "1-2 letras seguidas de 6 numeros (ejemplo: A123456)");

        public static bool EstaDisponible(string? estado) =>
            string.Equals(estado, "Disponible", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(estado, "Activo", StringComparison.OrdinalIgnoreCase);

        public static Result Validar(Autobus? autobus)
        {
            if (autobus is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("autobus"));
            }

            return ValidationGeneral.Combinar(ValidarPlaca(autobus.Placa),
                ValidationGeneral.EnteroPositivo(autobus.Capacidad, "capacidad"),
                ValidationGeneral.Requerido(autobus.Estado, "estado del autobus"),
                ValidationGeneral.Requerido(autobus.Modelo, "Modelo del autobus"),
                ValidationGeneral.Requerido(autobus.Marca, "Marca del autobus"));
        }

        public static Result ValidarParaAsignacion(Autobus? autobus)
        {
            var validacion = Validar(autobus);
            if (validacion.EsFallo)
            {
                return validacion;
            }

            if (!EstaDisponible(autobus!.Estado))
            {
                return Result.Fallo(DomainErrors.CatalogoTransporte.AutobusNoDisponible);
            }

            return ValidationGeneral.IdValido(autobus.Id, "autobus");
        }
    }
}