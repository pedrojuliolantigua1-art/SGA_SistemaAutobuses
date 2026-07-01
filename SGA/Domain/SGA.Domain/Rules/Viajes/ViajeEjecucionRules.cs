using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules
{
    //Aqui ya se validan las reglas de negocio que tienen que ver con la ejecucion de un viaje ya de ese viaje que ya esta programado
    //y se va a ejecutar, las incidencias entran aqui tambien porque son parte de la ejecucion del viaje, no de la planificacion
    public static class ViajeEjecucionRules
    {
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

            var fechaValida = ValidationGeneral.FechaDefinida(fechaHora, "inicio del viaje");

            if (fechaValida.EsFallo)
            {
                return fechaValida;
            }

            if (!ViajeEspecificaciones.PuedeIniciar(viaje))
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

            var fechaValida = ValidationGeneral.FechaDefinida(fechaHora, "fin del viaje");

            if (fechaValida.EsFallo)
            {
                return fechaValida;
            }

            if (!ViajeEspecificaciones.PuedeFinalizar(viaje) ||
                viaje.HoraInicioReal is not null && fechaHora < viaje.HoraInicioReal)
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

            var motivoValido = ValidationGeneral.RequeridoConLongitud(motivo, "motivo de cancelacion", min: 5, max: 250);

            if (motivoValido.EsFallo)
            {
                return motivoValido;
            }

            if (!ViajeEspecificaciones.PuedeCancelar(viaje))
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

            if (!ViajeEspecificaciones.PuedeMarcarRetrasado(viaje))
            {
                return Result.Fallo(DomainErrors.Viajes.EstadoInvalido);
            }

            viaje.Estado = EstadoViaje.Retrasado;
            return Result.Ok();
        }

        public static Result<Incidencia> ReportarIncidencia(
            Viaje? viaje, int conductorId, string? tipo, string? descripcion, DateTime fechaHora)
        {
            if (viaje is null)
            {
                return Result<Incidencia>.Fallo(DomainErrors.Viajes.ViajeRequerido);
            }

            if (viaje.ConductorId != conductorId)
            {
                return Result<Incidencia>.Fallo(DomainErrors.Viajes.ConductorNoAsignado);
            }

            if (!ViajeEspecificaciones.EstaEnEjecucion(viaje))
            {
                return Result<Incidencia>.Fallo(DomainErrors.Viajes.EstadoInvalido);
            }

            var validacion = ValidationGeneral.Combinar(
                ValidationGeneral.RequeridoConLongitud(tipo, "tipo de incidencia", min: 3, max: 50),
                ValidationGeneral.RequeridoConLongitud(descripcion, "descripcion de incidencia", min: 5, max: 500),
                ValidationGeneral.FechaDefinida(fechaHora, "incidencia"));

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
