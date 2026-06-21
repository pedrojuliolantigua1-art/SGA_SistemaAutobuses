using SGA.Domain.Entities.Transporte;
using SGA.Domain.Error;

namespace SGA.Domain.Rules
{
    public static class CatalogoTransporteRules
    {
        public static Result ValidarAutobus(Autobus? autobus)
        {
            if (autobus is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("autobus"));
            }

            return CommonValidationRules.Combinar(
                CommonValidationRules.Requerido(autobus.Placa, "placa"),
                CommonValidationRules.EnteroPositivo(autobus.Capacidad, "capacidad"),
                CommonValidationRules.Requerido(autobus.Estado, "estado del autobus"));
        }

        public static Result ValidarAutobusParaAsignacion(Autobus? autobus)
        {
            var validacion = ValidarAutobus(autobus);

            if (validacion.EsFallo)
            {
                return validacion;
            }

            if (!EstaDisponible(autobus!.Estado))
            {
                return Result.Fallo(DomainErrors.CatalogoTransporte.AutobusNoDisponible);
            }

            return CommonValidationRules.IdValido(autobus.Id, "autobus");
        }

        public static Result ValidarConductorParaAsignacion(Conductor? conductor)
        {
            if (conductor is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("conductor"));
            }

            var validacion = CommonValidationRules.Combinar(
                CommonValidationRules.IdValido(conductor.Id, "conductor"),
                CommonValidationRules.Requerido(conductor.CodigoInstitucional, "codigo institucional"),
                CommonValidationRules.Requerido(conductor.Nombre, "nombre del conductor"),
                CommonValidationRules.Requerido(conductor.Apellido, "apellido del conductor"),
                CommonValidationRules.Requerido(conductor.NumeroLicencia, "numero de licencia"));

            if (validacion.EsFallo)
            {
                return validacion;
            }

            return conductor.Disponible && EstaActivo(conductor.Estado)
                ? Result.Ok()
                : Result.Fallo(DomainErrors.CatalogoTransporte.ConductorNoDisponible);
        }

        public static Result ValidarRuta(Ruta? ruta, IEnumerable<Parada>? paradas = null)
        {
            if (ruta is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("ruta"));
            }

            var validacion = CommonValidationRules.Combinar(
                CommonValidationRules.IdValido(ruta.Id, "ruta"),
                CommonValidationRules.Requerido(ruta.Nombre, "nombre de la ruta"));

            if (validacion.EsFallo)
            {
                return validacion;
            }

            if (!ruta.Activa)
            {
                return Result.Fallo(DomainErrors.CatalogoTransporte.RutaInactiva);
            }

            if (paradas is null)
            {
                return Result.Ok();
            }

            var paradasLista = paradas.ToList();
            if (paradasLista.Count < 2)
            {
                return Result.Fallo(DomainErrors.CatalogoTransporte.RutaSinParadasSuficientes);
            }

            return paradasLista.Any(parada => parada.Orden <= 0)
                ? Result.Fallo(DomainErrors.General.IdInvalido("orden de parada"))
                : Result.Ok();
        }

        public static Result ValidarHorario(HorarioRuta? horario, Ruta? ruta = null)
        {
            if (horario is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("horario"));
            }

            var validacion = CommonValidationRules.Combinar(
                CommonValidationRules.IdValido(horario.Id, "horario"),
                CommonValidationRules.IdValido(horario.RutaId, "ruta del horario"));

            if (validacion.EsFallo)
            {
                return validacion;
            }

            if (!horario.Activo)
            {
                return Result.Fallo(DomainErrors.CatalogoTransporte.HorarioInactivo);
            }

            if (horario.HoraSalida == horario.HoraLlegadaEstimada)
            {
                return Result.Fallo(DomainErrors.CatalogoTransporte.HorarioInvalido);
            }

            if (ruta is not null && horario.RutaId != ruta.Id)
            {
                return Result.Fallo(DomainErrors.CatalogoTransporte.HorarioNoPerteneceRuta);
            }

            return Result.Ok();
        }

        public static bool EstaActivo(string? estado)
        {
            return string.Equals(estado, "Activo", StringComparison.OrdinalIgnoreCase);
        }

        public static bool EstaDisponible(string? estado)
        {
            return string.Equals(estado, "Disponible", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(estado, "Activo", StringComparison.OrdinalIgnoreCase);
        }
    }
}
