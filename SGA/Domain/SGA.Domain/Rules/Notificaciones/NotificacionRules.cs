using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Entities.Notificaciones;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules.Notificaciones
{
    public static class NotificacionRules
    {
        public const int DiasAnticipacionVencimiento = 3;

        public static bool EsCambioEstadoNotificable(EstadoViaje estado) =>
            estado is EstadoViaje.Retrasado or EstadoViaje.Cancelado;

        public static bool TicketProximoAVencer(
            TicketMensual ticket,
            DateTime fecha,
            int diasAnticipacion = DiasAnticipacionVencimiento)
        {
            var diasRestantes = (ticket.FechaFin.Date - fecha.Date).TotalDays;
            return diasRestantes >= 0 && diasRestantes <= diasAnticipacion;
        }

        public static bool SaldoInsuficiente(
            TarjetaRecargable tarjeta,
            decimal costoViaje = AutorizacionRules.CostoViajePredeterminado) =>
            tarjeta.SaldoDisponible < costoViaje;

        public static Result<Notificacion> CrearCambioEstadoViaje(
            int usuarioTransporteId,
            int viajeId,
            EstadoViaje estadoAnterior,
            EstadoViaje estadoNuevo,
            DateTime fechaHora)
        {
            if (!EsCambioEstadoNotificable(estadoNuevo))
            {
                return Result<Notificacion>.Fallo(DomainErrors.Notificaciones.EventoNoNotificable);
            }

            return Crear(
                usuarioTransporteId,
                "CambioEstadoViaje",
                $"Cambio de estado del viaje {viajeId}",
                $"El viaje cambio de {estadoAnterior} a {estadoNuevo}.",
                fechaHora);
        }

        public static Result<Notificacion> CrearVencimientoTicket(
            TicketMensual ticket,
            DateTime fechaHora,
            int diasAnticipacion = DiasAnticipacionVencimiento)
        {
            if (!TicketProximoAVencer(ticket, fechaHora, diasAnticipacion))
            {
                return Result<Notificacion>.Fallo(DomainErrors.Notificaciones.EventoNoNotificable);
            }

            return Crear(
                ticket.UsuarioTransporteId,
                "VencimientoTicket",
                "Ticket mensual proximo a vencer",
                $"Tu ticket mensual vence el {ticket.FechaFin:yyyy-MM-dd}.",
                fechaHora);
        }

        public static Result<Notificacion> CrearSaldoInsuficiente(
            TarjetaRecargable tarjeta,
            DateTime fechaHora,
            decimal costoViaje = AutorizacionRules.CostoViajePredeterminado)
        {
            if (!SaldoInsuficiente(tarjeta, costoViaje))
            {
                return Result<Notificacion>.Fallo(DomainErrors.Notificaciones.EventoNoNotificable);
            }

            return Crear(
                tarjeta.UsuarioTransporteId,
                "SaldoInsuficiente",
                "Saldo insuficiente",
                $"Saldo disponible: {tarjeta.SaldoDisponible}. Costo requerido: {costoViaje}.",
                fechaHora);
        }

        public static Result<Notificacion> CrearIncidenciaAdministrador(
            int administradorTransporteId,
            Incidencia incidencia,
            DateTime fechaHora)
        {
            var descripcionValida = ValidationGeneral.Requerido(
                incidencia.Descripcion,
                "descripcion de incidencia");

            if (descripcionValida.EsFallo)
            {
                return Result<Notificacion>.Fallo(descripcionValida.Error!);
            }

            return Crear(
                administradorTransporteId,
                "IncidenciaViaje",
                $"Incidencia en viaje {incidencia.ViajeId}",
                incidencia.Descripcion!,
                fechaHora);
        }

        private static Result<Notificacion> Crear(
            int usuarioTransporteId,
            string tipo,
            string titulo,
            string mensaje,
            DateTime fechaHora)
        {
            var validacion = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(usuarioTransporteId, "usuario destinatario"),
                ValidationGeneral.Requerido(tipo, "tipo de notificacion"),
                ValidationGeneral.Requerido(titulo, "titulo de notificacion"),
                ValidationGeneral.Requerido(mensaje, "mensaje de notificacion"),
                ValidationGeneral.FechaDefinida(fechaHora, "notificacion"));

            if (validacion.EsFallo)
            {
                return Result<Notificacion>.Fallo(validacion.Error!);
            }

            return Result<Notificacion>.Ok(new Notificacion
            {
                UsuarioTransporteId = usuarioTransporteId,
                Tipo = tipo,
                Titulo = titulo,
                Mensaje = mensaje,
                FechaHora = fechaHora,
                Leida = false
            });
        }
    }
}
