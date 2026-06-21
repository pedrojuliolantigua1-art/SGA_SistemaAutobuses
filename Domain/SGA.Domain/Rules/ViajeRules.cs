using SGA.Domain.Entities.Transporte;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Error;

namespace SGA.Domain.Rules
{
    public static class ViajeRules
    {
        public static bool PuedeIniciar(Viaje viaje) =>
            viaje.Estado == EstadoViaje.Programado ||
            viaje.Estado == EstadoViaje.Retrasado && viaje.HoraInicioReal is null;

        public static bool PuedeFinalizar(Viaje viaje) =>
            viaje.Estado == EstadoViaje.EnCurso ||
            viaje.Estado == EstadoViaje.Retrasado && viaje.HoraInicioReal is not null;

        public static bool PuedeCancelar(Viaje viaje) =>
            viaje.Estado == EstadoViaje.Programado ||
            viaje.Estado == EstadoViaje.Retrasado && viaje.HoraInicioReal is null;

        public static bool PuedeMarcarRetrasado(Viaje viaje) =>
            viaje.Estado is EstadoViaje.Programado or EstadoViaje.EnCurso;

        public static bool TieneCupoDisponible(Viaje viaje) =>
            viaje.CupoActual < viaje.CapacidadMaxima;

        public static bool EstaEnEjecucion(Viaje viaje) =>
            viaje.Estado == EstadoViaje.EnCurso ||
            viaje.Estado == EstadoViaje.Retrasado && viaje.HoraInicioReal is not null;

        public static bool EsActivoParaAsignacion(Viaje viaje) =>
            viaje.Estado is EstadoViaje.Programado or EstadoViaje.EnCurso or EstadoViaje.Retrasado;

        public static Result ValidarPlanificacionBase(Viaje? viaje)
        {
            if (viaje is null)
            {
                return Result.Fallo(DomainErrors.Viajes.ViajeRequerido);
            }

            var validacion = CommonValidationRules.Combinar(
                CommonValidationRules.IdValido(viaje.RutaId, "ruta"),
                CommonValidationRules.IdValido(viaje.HorarioRutaId, "horario"),
                CommonValidationRules.IdValido(viaje.AutobusId, "autobus"),
                CommonValidationRules.IdValido(viaje.ConductorId, "conductor"),
                CommonValidationRules.FechaDefinida(viaje.Fecha, "viaje"));

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

        public static Result ValidarPlanificacion(
            Viaje? viaje,
            Ruta? ruta,
            HorarioRuta? horario,
            Autobus? autobus,
            Conductor? conductor,
            IEnumerable<Viaje>? viajesExistentes = null)
        {
            var baseValida = ValidarPlanificacionBase(viaje);

            if (baseValida.EsFallo)
            {
                return baseValida;
            }

            var catalogoValido = CommonValidationRules.Combinar(
                CatalogoTransporteRules.ValidarRuta(ruta),
                CatalogoTransporteRules.ValidarHorario(horario, ruta),
                CatalogoTransporteRules.ValidarAutobusParaAsignacion(autobus),
                CatalogoTransporteRules.ValidarConductorParaAsignacion(conductor));

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

        public static Result<Viaje> CrearViajePlanificado(
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

            var validacion = ValidarPlanificacion(viaje, ruta, horario, autobus, conductor, viajesExistentes);

            return validacion.EsFallo
                ? Result<Viaje>.Fallo(validacion.Error!)
                : Result<Viaje>.Ok(viaje);
        }

        public static bool HayConflictoAutobus(
            IEnumerable<Viaje>? viajes,
            int autobusId,
            int horarioRutaId,
            DateTime fecha,
            int viajeIdExcluir = 0)
        {
            return viajes?.Any(viaje =>
                viaje.Id != viajeIdExcluir &&
                viaje.AutobusId == autobusId &&
                viaje.HorarioRutaId == horarioRutaId &&
                viaje.Fecha.Date == fecha.Date &&
                EsActivoParaAsignacion(viaje)) ?? false;
        }

        public static bool HayConflictoConductor(
            IEnumerable<Viaje>? viajes,
            int conductorId,
            int horarioRutaId,
            DateTime fecha,
            int viajeIdExcluir = 0)
        {
            return viajes?.Any(viaje =>
                viaje.Id != viajeIdExcluir &&
                viaje.ConductorId == conductorId &&
                viaje.HorarioRutaId == horarioRutaId &&
                viaje.Fecha.Date == fecha.Date &&
                EsActivoParaAsignacion(viaje)) ?? false;
        }

        public static bool PuedeTransicionar(Viaje viaje, EstadoViaje nuevoEstado)
        {
            return (viaje.Estado, nuevoEstado) switch
            {
                (EstadoViaje.Programado, EstadoViaje.EnCurso) => true,
                (EstadoViaje.Programado, EstadoViaje.Cancelado) => true,
                (EstadoViaje.Programado, EstadoViaje.Retrasado) => true,
                (EstadoViaje.Retrasado, EstadoViaje.EnCurso) when viaje.HoraInicioReal is null => true,
                (EstadoViaje.Retrasado, EstadoViaje.Cancelado) when viaje.HoraInicioReal is null => true,
                (EstadoViaje.EnCurso, EstadoViaje.Finalizado) => true,
                (EstadoViaje.EnCurso, EstadoViaje.Retrasado) => true,
                (EstadoViaje.Retrasado, EstadoViaje.Finalizado) when viaje.HoraInicioReal is not null => true,
                _ => false
            };
        }

        public static Result CambiarEstado(Viaje? viaje, EstadoViaje nuevoEstado)
        {
            if (viaje is null)
            {
                return Result.Fallo(DomainErrors.Viajes.ViajeRequerido);
            }

            if (!PuedeTransicionar(viaje, nuevoEstado))
            {
                return Result.Fallo(DomainErrors.Viajes.EstadoInvalido);
            }

            viaje.Estado = nuevoEstado;
            return Result.Ok();
        }

        public static Result Iniciar(Viaje? viaje, int conductorId, DateTime fechaHora)
        {
            if (viaje is null)
            {
                return Result.Fallo(DomainErrors.Viajes.ViajeRequerido);
            }

            if (viaje.ConductorId != conductorId)
            {
                return Result.Fallo(DomainErrors.Viajes.ConductorNoAsignado);
            }

            var fechaValida = CommonValidationRules.FechaDefinida(fechaHora, "inicio del viaje");

            if (fechaValida.EsFallo)
            {
                return fechaValida;
            }

            if (!PuedeIniciar(viaje))
            {
                return Result.Fallo(DomainErrors.Viajes.EstadoInvalido);
            }

            viaje.HoraInicioReal = fechaHora;
            viaje.Estado = EstadoViaje.EnCurso;
            return Result.Ok();
        }

        public static Result Finalizar(Viaje? viaje, int conductorId, DateTime fechaHora)
        {
            if (viaje is null)
            {
                return Result.Fallo(DomainErrors.Viajes.ViajeRequerido);
            }

            if (viaje.ConductorId != conductorId)
            {
                return Result.Fallo(DomainErrors.Viajes.ConductorNoAsignado);
            }

            var fechaValida = CommonValidationRules.FechaDefinida(fechaHora, "fin del viaje");

            if (fechaValida.EsFallo)
            {
                return fechaValida;
            }

            if (!PuedeFinalizar(viaje) || viaje.HoraInicioReal is not null && fechaHora < viaje.HoraInicioReal)
            {
                return Result.Fallo(DomainErrors.Viajes.EstadoInvalido);
            }

            viaje.HoraFinReal = fechaHora;
            viaje.Estado = EstadoViaje.Finalizado;
            return Result.Ok();
        }

        public static Result Cancelar(Viaje? viaje, string? motivo)
        {
            if (viaje is null)
            {
                return Result.Fallo(DomainErrors.Viajes.ViajeRequerido);
            }

            var motivoValido = CommonValidationRules.Requerido(motivo, "motivo de cancelacion");

            if (motivoValido.EsFallo)
            {
                return Result.Fallo(DomainErrors.Viajes.MotivoCancelacionRequerido);
            }

            if (!PuedeCancelar(viaje))
            {
                return Result.Fallo(DomainErrors.Viajes.EstadoInvalido);
            }

            viaje.Estado = EstadoViaje.Cancelado;
            return Result.Ok();
        }

        public static Result MarcarRetrasado(Viaje? viaje)
        {
            if (viaje is null)
            {
                return Result.Fallo(DomainErrors.Viajes.ViajeRequerido);
            }

            if (!PuedeMarcarRetrasado(viaje))
            {
                return Result.Fallo(DomainErrors.Viajes.EstadoInvalido);
            }

            viaje.Estado = EstadoViaje.Retrasado;
            return Result.Ok();
        }

        public static Result<Incidencia> ReportarIncidencia(
            Viaje? viaje,
            int conductorId,
            string? tipo,
            string? descripcion,
            DateTime fechaHora)
        {
            if (viaje is null)
            {
                return Result<Incidencia>.Fallo(DomainErrors.Viajes.ViajeRequerido);
            }

            if (viaje.ConductorId != conductorId)
            {
                return Result<Incidencia>.Fallo(DomainErrors.Viajes.ConductorNoAsignado);
            }

            if (!EstaEnEjecucion(viaje))
            {
                return Result<Incidencia>.Fallo(DomainErrors.Viajes.EstadoInvalido);
            }

            var validacion = CommonValidationRules.Combinar(
                CommonValidationRules.Requerido(tipo, "tipo de incidencia"),
                CommonValidationRules.Requerido(descripcion, "descripcion de incidencia"),
                CommonValidationRules.FechaDefinida(fechaHora, "incidencia"));

            if (validacion.EsFallo)
            {
                return Result<Incidencia>.Fallo(validacion.Error!);
            }

            if (string.Equals(tipo, "Retraso", StringComparison.OrdinalIgnoreCase))
            {
                viaje.Estado = EstadoViaje.Retrasado;
            }

            return Result<Incidencia>.Ok(new Incidencia
            {
                ViajeId = viaje.Id,
                ConductorId = conductorId,
                Tipo = tipo,
                Descripcion = descripcion,
                FechaHora = fechaHora
            });
        }
    }
}
