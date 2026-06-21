using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Enum;
using SGA.Domain.Error;

namespace SGA.Domain.Rules
{
    public static class AutorizacionRules
    {
        public const decimal CostoViajePredeterminado = 1m;

        public static bool RequierePago() => true;

        public static bool SaldoInicialValido(decimal saldo) => saldo > 0;

        public static bool MontoRecargaValido(decimal monto) => monto > 0;

        public static bool TicketExpirado(TicketMensual ticket) =>
            DateTime.UtcNow.Date > ticket.FechaFin.Date;

        public static DateTime CalcularFechaFin(DateTime fechaInicio) =>
            fechaInicio.Date.AddMonths(1).AddDays(-1);

        public static Result ValidarPagoRegistrado(PagoTransporte? pago, int usuarioTransporteId)
        {
            if (pago is null)
            {
                return Result.Fallo(DomainErrors.Autorizaciones.PagoRequerido);
            }

            var validacion = CommonValidationRules.Combinar(
                CommonValidationRules.IdValido(pago.Id, "pago"),
                CommonValidationRules.IdValido(pago.UsuarioTransporteId, "usuario del pago"),
                CommonValidationRules.MontoPositivo(pago.Monto, "pago"),
                CommonValidationRules.Requerido(pago.TipoPago, "tipo de pago"),
                CommonValidationRules.Requerido(pago.Estado, "estado del pago"),
                CommonValidationRules.Requerido(pago.NumeroComprobante, "numero de comprobante"),
                CommonValidationRules.FechaDefinida(pago.FechaHora, "pago"));

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

        public static Result ValidarEmisionTicket(
            PagoTransporte? pago,
            int usuarioTransporteId,
            DateTime fechaInicio,
            TicketMensual? ticketActivo = null)
        {
            var pagoValido = ValidarPagoRegistrado(pago, usuarioTransporteId);

            if (pagoValido.EsFallo)
            {
                return pagoValido;
            }

            var fechaFin = CalcularFechaFin(fechaInicio);
            var rangoValido = CommonValidationRules.RangoFechasValido(fechaInicio.Date, fechaFin, "ticket mensual");

            if (rangoValido.EsFallo)
            {
                return rangoValido;
            }

            return ticketActivo is not null && TicketVigente(ticketActivo, fechaInicio)
                ? Result.Fallo(DomainErrors.Autorizaciones.TicketActivoExistente)
                : Result.Ok();
        }

        public static Result<TicketMensual> CrearTicketMensual(
            PagoTransporte? pago,
            int usuarioTransporteId,
            DateTime fechaInicio,
            TicketMensual? ticketActivo = null,
            DateTime? fechaEmision = null)
        {
            var validacion = ValidarEmisionTicket(pago, usuarioTransporteId, fechaInicio, ticketActivo);

            if (validacion.EsFallo)
            {
                return Result<TicketMensual>.Fallo(validacion.Error!);
            }

            var inicio = fechaInicio.Date;

            return Result<TicketMensual>.Ok(new TicketMensual
            {
                UsuarioTransporteId = usuarioTransporteId,
                FechaEmision = fechaEmision ?? DateTime.UtcNow,
                FechaInicio = inicio,
                FechaFin = CalcularFechaFin(inicio),
                Estado = EstadoAutorizacion.Activa
            });
        }

        public static Result ValidarEmisionTarjeta(
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

            return CommonValidationRules.Combinar(
                CommonValidationRules.MontoPositivo(saldoInicial, "saldo inicial"),
                CommonValidationRules.Requerido(numeroTarjeta, "numero de tarjeta"));
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

        public static Result ValidarRecarga(
            TarjetaRecargable? tarjeta,
            PagoTransporte? pago,
            decimal monto)
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

            return CommonValidationRules.MontoPositivo(monto, "recarga");
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
                TicketMensual ticket => ValidarTicketVigente(ticket, fecha),
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

        public static bool TicketVigente(TicketMensual ticket, DateTime? fecha = null)
        {
            var fechaEvaluacion = (fecha ?? DateTime.UtcNow).Date;

            return ticket.Estado == EstadoAutorizacion.Activa &&
                   fechaEvaluacion >= ticket.FechaInicio.Date &&
                   fechaEvaluacion <= ticket.FechaFin.Date;
        }

        public static Result ValidarTicketVigente(TicketMensual? ticket, DateTime fecha)
        {
            if (ticket is null)
            {
                return Result.Fallo(DomainErrors.Accesos.AutorizacionRequerida);
            }

            return TicketVigente(ticket, fecha)
                ? Result.Ok()
                : Result.Fallo(DomainErrors.Autorizaciones.TicketVencido);
        }

        public static Result ValidarTarjetaConSaldo(
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

            var costoValido = CommonValidationRules.MontoPositivo(costoViaje, "costo del viaje");

            if (costoValido.EsFallo)
            {
                return costoValido;
            }

            return tarjeta.SaldoDisponible >= costoViaje
                ? Result.Ok()
                : Result.Fallo(DomainErrors.Autorizaciones.SaldoInsuficiente);
        }

        public static Result ValidarPermisoVigente(PermisoTransporte? permiso, DateTime fecha)
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
