using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Enum;
using SGA.Domain.Error;
using SGA.Domain.Validation;
using DomainError = SGA.Domain.Error.Error;

namespace SGA.Domain.Rules
{
    public static class AutorizacionRules
    {
        public const decimal CostoViajePredeterminado = 1m;

        public static DateTime CalcularFechaFin(DateTime fechaInicio) =>
            fechaInicio.Date.AddMonths(1).AddDays(-1);

        public static Result<TicketDiario> CrearTicketDiario(
            PagoTransporte? pago,
            int usuarioTransporteId,
            DateTime fechaInicio,
            TicketDiario? ticketActivo = null,
            DateTime? fechaEmision = null)
        {
            var validacion = ValidarEmisionTicket(pago, usuarioTransporteId, fechaInicio, ticketActivo);

            if (validacion.EsFallo)
            {
                return Result<TicketDiario>.Fallo(validacion.Error!);
            }

            var inicio = fechaInicio.Date;

            return Result<TicketDiario>.Ok(new TicketDiario
            {
                UsuarioTransporteId = usuarioTransporteId,
                FechaEmision = fechaEmision ?? DateTime.UtcNow,
                FechaInicio = inicio,
                FechaFin = CalcularFechaFin(inicio),
                Estado = EstadoAutorizacion.Activa
            });
        }

        public static Result<TarjetaRecargable> CrearTarjetaRecargable(
            PagoTransporte? pago,
            int usuarioTransporteId,
            decimal saldoInicial,
            string? numeroTarjeta,
            DateTime? fechaEmision = null)
        {
            var validacion = ValidarEmisionTarjeta(pago, usuarioTransporteId, saldoInicial, numeroTarjeta);

            if (validacion.EsFallo)
            {
                return Result<TarjetaRecargable>.Fallo(validacion.Error!);
            }

            return Result<TarjetaRecargable>.Ok(new TarjetaRecargable
            {
                UsuarioTransporteId = usuarioTransporteId,
                FechaEmision = fechaEmision ?? DateTime.UtcNow,
                NumeroTarjeta = numeroTarjeta,
                SaldoDisponible = saldoInicial,
                Estado = EstadoAutorizacion.Activa
            });
        }

        public static Result<PermisoTransporte> CrearPermiso(
            int usuarioTransporteId,
            string? condicionInstitucional,
            DateTime? fechaVencimiento,
            DateTime? fechaEmision = null)
        {
            var validacion = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(usuarioTransporteId, "usuario"),
                ValidarEmisionPermiso(condicionInstitucional, fechaVencimiento));

            if (validacion.EsFallo)
            {
                return Result<PermisoTransporte>.Fallo(validacion.Error!);
            }

            return Result<PermisoTransporte>.Ok(new PermisoTransporte
            {
                UsuarioTransporteId = usuarioTransporteId,
                FechaEmision = fechaEmision ?? DateTime.UtcNow,
                CondicionInstitucional = condicionInstitucional,
                FechaVencimiento = fechaVencimiento,
                Estado = EstadoAutorizacion.Activa
            });
        }

        public static Result ValidarRecarga(TarjetaRecargable? tarjeta, PagoTransporte? pago, decimal monto)
        {
            if (tarjeta is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("tarjeta recargable"));
            }

            if (tarjeta.Estado != EstadoAutorizacion.Activa)
            {
                return Result.Fallo(DomainErrors.Autorizaciones.TarjetaInactiva);
            }

            var pagoValido = ValidarPagoRegistrado(pago, tarjeta.UsuarioTransporteId);

            if (pagoValido.EsFallo)
            {
                return pagoValido;
            }

            return ValidationGeneral.MontoPositivo(monto, "recarga");
        }

        public static Result AplicarRecarga(TarjetaRecargable? tarjeta, PagoTransporte? pago, decimal monto)
        {
            var validacion = ValidarRecarga(tarjeta, pago, monto);

            if (validacion.EsFallo)
            {
                return validacion;
            }

            tarjeta!.SaldoDisponible += monto;
            return Result.Ok();
        }

        public static Result Anular(AutorizacionTransporte? autorizacion)
        {
            if (autorizacion is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("autorizacion"));
            }

            if (autorizacion.Estado == EstadoAutorizacion.Anulada)
            {
                return Result.Fallo(DomainErrors.Autorizaciones.AutorizacionYaAnulada);
            }

            autorizacion.Estado = EstadoAutorizacion.Anulada;
            return Result.Ok();
        }

        public static bool TicketVigente(TicketDiario ticket, DateTime? fecha = null)
        {
            var fechaEvaluacion = (fecha ?? DateTime.UtcNow).Date;

            return ticket.Estado == EstadoAutorizacion.Activa &&
                   fechaEvaluacion >= ticket.FechaInicio.Date &&
                   fechaEvaluacion <= ticket.FechaFin.Date;
        }

        public static Result ValidarAutorizacionParaAcceso(
            AutorizacionTransporte? autorizacion,
            DateTime fecha,
            decimal costoViaje = CostoViajePredeterminado)
        {
            if (autorizacion is null)
            {
                return Result.Fallo(DomainErrors.Accesos.AutorizacionRequerida);
            }

            return autorizacion switch
            {
                TicketDiario ticket => ValidarTicketVigente(ticket, fecha),
                TarjetaRecargable tarjeta => ValidarTarjetaConSaldo(tarjeta, costoViaje),
                PermisoTransporte permiso => ValidarPermisoVigente(permiso, fecha),
                _ => Result.Fallo(DomainErrors.Autorizaciones.TipoNoSoportado)
            };
        }

        public static Result ConsumirAutorizacion(
            AutorizacionTransporte? autorizacion,
            DateTime fechaHora,
            decimal costoViaje = CostoViajePredeterminado)
        {
            var validacion = ValidarAutorizacionParaAcceso(autorizacion, fechaHora, costoViaje);

            if (validacion.EsFallo)
            {
                return validacion;
            }

            if (autorizacion is TarjetaRecargable tarjeta)
            {
                tarjeta.SaldoDisponible -= costoViaje;

                if (tarjeta.SaldoDisponible <= 0)
                {
                    tarjeta.SaldoDisponible = 0;
                    tarjeta.Estado = EstadoAutorizacion.Consumida;
                }
            }

            return Result.Ok();
        }

        public static ResultadoAcceso ResultadoAccesoDesdeError(DomainError error)
        {
            if (error == DomainErrors.Autorizaciones.TicketVencido)
            {
                return ResultadoAcceso.AutorizacionVencida;
            }

            if (error == DomainErrors.Autorizaciones.SaldoInsuficiente)
            {
                return ResultadoAcceso.SaldoInsuficiente;
            }

            if (error == DomainErrors.Autorizaciones.TarjetaInactiva ||
                error == DomainErrors.Autorizaciones.TipoNoSoportado ||
                error == DomainErrors.Autorizaciones.AutorizacionNoPerteneceAlUsuario)
            {
                return ResultadoAcceso.AutorizacionInvalida;
            }

            if (error == DomainErrors.Accesos.AutorizacionRequerida)
            {
                return ResultadoAcceso.SinAutorizacion;
            }

            return ResultadoAcceso.Denegado;
        }

        private static Result ValidarPagoRegistrado(PagoTransporte? pago, int usuarioTransporteId)
        {
            if (pago is null)
            {
                return Result.Fallo(DomainErrors.Autorizaciones.PagoRequerido);
            }

            var validacion = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(pago.Id, "pago"),
                ValidationGeneral.IdValido(pago.UsuarioTransporteId, "usuario del pago"),
                ValidationGeneral.MontoPositivo(pago.Monto, "pago"),
                ValidationGeneral.Requerido(pago.TipoPago, "tipo de pago"),
                ValidationGeneral.Requerido(pago.Estado, "estado del pago"),
                ValidationGeneral.Requerido(pago.NumeroComprobante, "numero de comprobante"),
                ValidationGeneral.FechaDefinida(pago.FechaHora, "pago"));

            if (validacion.EsFallo)
            {
                return validacion;
            }

            if (pago.UsuarioTransporteId != usuarioTransporteId)
            {
                return Result.Fallo(DomainErrors.Autorizaciones.PagoNoPerteneceAlUsuario);
            }

            return EsPagoValido(pago.Estado)
                ? Result.Ok()
                : Result.Fallo(DomainErrors.Autorizaciones.PagoNoVerificable);
        }

        private static Result ValidarEmisionTicket(
            PagoTransporte? pago,
            int usuarioTransporteId,
            DateTime fechaInicio,
            TicketDiario? ticketActivo)
        {
            var pagoValido = ValidarPagoRegistrado(pago, usuarioTransporteId);

            if (pagoValido.EsFallo)
            {
                return pagoValido;
            }

            var fechaFin = CalcularFechaFin(fechaInicio);
            var rangoValido = ValidationGeneral.RangoFechasValido(fechaInicio.Date, fechaFin, "ticket mensual");

            if (rangoValido.EsFallo)
            {
                return rangoValido;
            }

            return ticketActivo is not null && TicketVigente(ticketActivo, fechaInicio)
                ? Result.Fallo(DomainErrors.Autorizaciones.TicketActivoExistente)
                : Result.Ok();
        }

        private static Result ValidarEmisionTarjeta(
            PagoTransporte? pago,
            int usuarioTransporteId,
            decimal saldoInicial,
            string? numeroTarjeta)
        {
            var pagoValido = ValidarPagoRegistrado(pago, usuarioTransporteId);

            if (pagoValido.EsFallo)
            {
                return pagoValido;
            }

            return ValidationGeneral.Combinar(
                ValidationGeneral.MontoPositivo(saldoInicial, "saldo inicial"),
                ValidationGeneral.Requerido(numeroTarjeta, "numero de tarjeta"));
        }

        private static Result ValidarEmisionPermiso(string? condicionInstitucional, DateTime? fechaVencimiento)
        {
            return ValidationGeneral.Combinar(
                ValidationGeneral.RequeridoConLongitud(condicionInstitucional, "condicion institucional", min: 3, max: 200),
                fechaVencimiento is not null && fechaVencimiento.Value.Date < DateTime.UtcNow.Date
                    ? Result.Fallo(DomainErrors.Autorizaciones.FechaVencimientoEnElPasado)
                    : Result.Ok());
        }

        private static Result ValidarTicketVigente(TicketDiario? ticket, DateTime fecha)
        {
            if (ticket is null)
            {
                return Result.Fallo(DomainErrors.Accesos.AutorizacionRequerida);
            }

            return TicketVigente(ticket, fecha)
                ? Result.Ok()
                : Result.Fallo(DomainErrors.Autorizaciones.TicketVencido);
        }

        private static Result ValidarTarjetaConSaldo(
            TarjetaRecargable? tarjeta,
            decimal costoViaje = CostoViajePredeterminado)
        {
            if (tarjeta is null)
            {
                return Result.Fallo(DomainErrors.Accesos.AutorizacionRequerida);
            }

            if (tarjeta.Estado != EstadoAutorizacion.Activa)
            {
                return Result.Fallo(DomainErrors.Autorizaciones.TarjetaInactiva);
            }

            var costoValido = ValidationGeneral.MontoPositivo(costoViaje, "costo del viaje");

            if (costoValido.EsFallo)
            {
                return costoValido;
            }

            return tarjeta.SaldoDisponible >= costoViaje
                ? Result.Ok()
                : Result.Fallo(DomainErrors.Autorizaciones.SaldoInsuficiente);
        }

        private static Result ValidarPermisoVigente(PermisoTransporte? permiso, DateTime fecha)
        {
            if (permiso is null)
            {
                return Result.Fallo(DomainErrors.Accesos.AutorizacionRequerida);
            }

            if (permiso.Estado != EstadoAutorizacion.Activa)
            {
                return Result.Fallo(DomainErrors.Autorizaciones.TipoNoSoportado);
            }

            return permiso.FechaVencimiento is null || fecha.Date <= permiso.FechaVencimiento.Value.Date
                ? Result.Ok()
                : Result.Fallo(DomainErrors.Autorizaciones.TicketVencido);
        }

        private static bool EsPagoValido(string? estado)
        {
            return string.Equals(estado, "Registrado", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(estado, "Aprobado", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(estado, "Pagado", StringComparison.OrdinalIgnoreCase);
        }
    }
}
