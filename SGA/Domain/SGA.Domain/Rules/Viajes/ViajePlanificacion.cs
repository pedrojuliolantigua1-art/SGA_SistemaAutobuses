
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules
{
    // Aqui veo que tenga que ver con la asignacion de ruta, horario,el autobus y un conductor a un viaje tipo si el autobus o el conductor
    // ya tienen un viaje asignado para la misma fecha y horario 
    public static class ViajePlanificacionRules
    {
        public static Result ValidarDatosBase(Viaje? viaje)
        {
            if (viaje is null)
            {
                return Result.Fallo(DomainErrors.Viajes.ViajeRequerido);
            }

            var validacion = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(viaje.RutaId, "ruta"),
                ValidationGeneral.IdValido(viaje.HorarioRutaId, "horario"),
                ValidationGeneral.IdValido(viaje.AutobusId, "autobus"),
                ValidationGeneral.IdValido(viaje.ConductorId, "conductor"),
                ValidationGeneral.FechaDefinida(viaje.Fecha, "viaje"));

            if (validacion.EsFallo)
            {
                return validacion;
            }

            if (viaje.CapacidadMaxima <= 0)
            {
                return Result.Fallo(DomainErrors.Viajes.CapacidadInvalida);
            }

            return viaje.CupoActual < 0 || viaje.CupoActual > viaje.CapacidadMaxima
                ? Result.Fallo(DomainErrors.Viajes.CupoInvalido)
                : Result.Ok();
        }

        public static Result Validar(
            Viaje? viaje,
            Ruta? ruta,
            HorarioRuta? horario,
            Autobus? autobus,
            Conductor? conductor,
            IEnumerable<Viaje>? viajesExistentes = null)
        {
            var baseValida = ValidarDatosBase(viaje);

            if (baseValida.EsFallo)
            {
                return baseValida;
            }

            var catalogoValido = ValidationGeneral.Combinar(
                RutaRules.Validar(ruta),
                HorarioRules.Validar(horario, ruta),
                AutobusRules.ValidarParaAsignacion(autobus),
                ConductorRules.ValidarParaAsignacion(conductor));

            if (catalogoValido.EsFallo)
            {
                return catalogoValido;
            }

            if (viaje!.RutaId != ruta!.Id ||
                viaje.HorarioRutaId != horario!.Id ||
                viaje.AutobusId != autobus!.Id ||
                viaje.ConductorId != conductor!.Id)
            {
                return Result.Fallo(DomainErrors.Viajes.PlanificacionInvalida);
            }

            if (HayConflictoAutobus(viajesExistentes, viaje.AutobusId, viaje.HorarioRutaId, viaje.Fecha, viaje.Id))
            {
                return Result.Fallo(DomainErrors.Viajes.ConflictoAutobus);
            }

            return HayConflictoConductor(viajesExistentes, viaje.ConductorId, viaje.HorarioRutaId, viaje.Fecha, viaje.Id)
                ? Result.Fallo(DomainErrors.Viajes.ConflictoConductor)
                : Result.Ok();
        }

        public static Result<Viaje> Crear(
            Ruta? ruta,
            HorarioRuta? horario,
            Autobus? autobus,
            Conductor? conductor,
            DateTime fecha,
            IEnumerable<Viaje>? viajesExistentes = null)
        {
            if (ruta is null || horario is null || autobus is null || conductor is null)
            {
                return Result<Viaje>.Fallo(DomainErrors.Viajes.PlanificacionInvalida);
            }

            var viaje = new Viaje
            {
                RutaId = ruta.Id,
                HorarioRutaId = horario.Id,
                AutobusId = autobus.Id,
                ConductorId = conductor.Id,
                Fecha = fecha.Date,
                Estado = EstadoViaje.Programado,
                CupoActual = 0,
                CapacidadMaxima = autobus.Capacidad
            };

            var validacion = Validar(viaje, ruta, horario, autobus, conductor, viajesExistentes);

            return validacion.EsFallo
                ? Result<Viaje>.Fallo(validacion.Error!)
                : Result<Viaje>.Ok(viaje);
        }

        public static bool HayConflictoAutobus(
            IEnumerable<Viaje>? viajes, int autobusId, int horarioRutaId, DateTime fecha, int viajeIdExcluir = 0)
        {
            return viajes?.Any(viaje =>
                viaje.Id != viajeIdExcluir &&
                viaje.AutobusId == autobusId &&
                viaje.HorarioRutaId == horarioRutaId &&
                viaje.Fecha.Date == fecha.Date &&
                ViajeEspecificaciones.EsActivoParaAsignacion(viaje)) ?? false;
        }

        public static bool HayConflictoConductor(
            IEnumerable<Viaje>? viajes, int conductorId, int horarioRutaId, DateTime fecha, int viajeIdExcluir = 0)
        {
            return viajes?.Any(viaje =>
                viaje.Id != viajeIdExcluir &&
                viaje.ConductorId == conductorId &&
                viaje.HorarioRutaId == horarioRutaId &&
                viaje.Fecha.Date == fecha.Date &&
                ViajeEspecificaciones.EsActivoParaAsignacion(viaje)) ?? false;
        }
    }
}
