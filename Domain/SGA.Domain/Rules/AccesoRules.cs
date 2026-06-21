using SGA.Domain.Entities.Accesos;
using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Error;
using DomainError = SGA.Domain.Error.Error;

namespace SGA.Domain.Rules
{
    public static class AccesoRules
    {
        public static bool ViajeDisponibleParaAbordaje(Viaje viaje) =>
            ViajeRules.EstaEnEjecucion(viaje);

        public static bool TicketVigente(TicketMensual ticket) =>
            AutorizacionRules.TicketVigente(ticket);

        public static bool TarjetaConSaldo(TarjetaRecargable tarjeta) =>
            tarjeta.Estado == EstadoAutorizacion.Activa &&
            tarjeta.SaldoDisponible >= AutorizacionRules.CostoViajePredeterminado;

        public static bool HayCupoDisponible(Viaje viaje) =>
            ViajeRules.TieneCupoDisponible(viaje);

        public static ResultadoAcceso Evaluar(AutorizacionTransporte? autorizacion, Viaje viaje)
        {
            if (!ViajeDisponibleParaAbordaje(viaje))
            {
                return ResultadoAcceso.ViajeNoDisponible;
            }

            if (autorizacion is null)
            {
                return ResultadoAcceso.SinAutorizacion;
            }

            var autorizacionValida = AutorizacionRules.ValidarAutorizacionParaAcceso(
                autorizacion,
                DateTime.UtcNow);

            if (autorizacionValida.EsFallo)
            {
                return MapearResultado(autorizacionValida.Error!);
            }

            return HayCupoDisponible(viaje)
                ? ResultadoAcceso.Permitido
                : ResultadoAcceso.SinCupo;
        }

        public static Result ValidarAcceso(
            UsuarioTransporte? usuario,
            AutorizacionTransporte? autorizacion,
            Viaje? viaje,
            DateTime fechaHora,
            decimal costoViaje = AutorizacionRules.CostoViajePredeterminado)
        {
            if (usuario is null)
            {
                return Result.Fallo(DomainErrors.Accesos.UsuarioRequerido);
            }

            if (!CatalogoTransporteRules.EstaActivo(usuario.Estado))
            {
                return Result.Fallo(DomainErrors.Accesos.UsuarioInactivo);
            }

            if (viaje is null)
            {
                return Result.Fallo(DomainErrors.Accesos.ViajeRequerido);
            }

            if (!ViajeDisponibleParaAbordaje(viaje))
            {
                return Result.Fallo(DomainErrors.Accesos.ViajeNoDisponible);
            }

            if (autorizacion is null)
            {
                return Result.Fallo(DomainErrors.Accesos.AutorizacionRequerida);
            }

            if (autorizacion.UsuarioTransporteId != usuario.Id)
            {
                return Result.Fallo(DomainErrors.Autorizaciones.AutorizacionNoPerteneceAlUsuario);
            }

            var autorizacionValida = AutorizacionRules.ValidarAutorizacionParaAcceso(
                autorizacion,
                fechaHora,
                costoViaje);

            if (autorizacionValida.EsFallo)
            {
                return autorizacionValida;
            }

            return HayCupoDisponible(viaje)
                ? Result.Ok()
                : Result.Fallo(DomainErrors.Accesos.SinCupo);
        }

        public static EvaluacionAcceso EvaluarAcceso(
            UsuarioTransporte? usuario,
            AutorizacionTransporte? autorizacion,
            Viaje? viaje,
            DateTime fechaHora,
            decimal costoViaje = AutorizacionRules.CostoViajePredeterminado)
        {
            var validacion = ValidarAcceso(usuario, autorizacion, viaje, fechaHora, costoViaje);

            return validacion.EsExitoso
                ? EvaluacionAcceso.Aprobada()
                : EvaluacionAcceso.Rechazada(MapearResultado(validacion.Error!), validacion.Error!.Mensaje);
        }

        public static Result<RegistroUsoTransporte> CrearRegistroIntento(
            int usuarioTransporteId,
            int viajeId,
            int? autorizacionTransporteId,
            int validadoPorUsuarioId,
            ResultadoAcceso resultado,
            string? motivoRechazo,
            DateTime fechaHora)
        {
            var validacion = CommonValidationRules.Combinar(
                CommonValidationRules.IdValido(usuarioTransporteId, "usuario"),
                CommonValidationRules.IdValido(viajeId, "viaje"),
                CommonValidationRules.IdValido(validadoPorUsuarioId, "usuario validador"),
                CommonValidationRules.FechaDefinida(fechaHora, "intento de acceso"));

            if (validacion.EsFallo)
            {
                return Result<RegistroUsoTransporte>.Fallo(validacion.Error!);
            }

            if (resultado != ResultadoAcceso.Permitido && string.IsNullOrWhiteSpace(motivoRechazo))
            {
                return Result<RegistroUsoTransporte>.Fallo(DomainErrors.General.CampoRequerido("motivo de rechazo"));
            }

            return Result<RegistroUsoTransporte>.Ok(new RegistroUsoTransporte
            {
                UsuarioTransporteId = usuarioTransporteId,
                ViajeId = viajeId,
                AutorizacionTransporteId = autorizacionTransporteId,
                ResultadoAcceso = resultado,
                MotivoRechazo = motivoRechazo,
                FechaHora = fechaHora,
                ValidadoPorUsuarioId = validadoPorUsuarioId
            });
        }

        public static Result<RegistroUsoTransporte> CrearRegistroDesdeEvaluacion(
            UsuarioTransporte? usuario,
            AutorizacionTransporte? autorizacion,
            Viaje? viaje,
            int validadoPorUsuarioId,
            DateTime fechaHora,
            decimal costoViaje = AutorizacionRules.CostoViajePredeterminado)
        {
            if (usuario is null)
            {
                return Result<RegistroUsoTransporte>.Fallo(DomainErrors.Accesos.UsuarioRequerido);
            }

            if (viaje is null)
            {
                return Result<RegistroUsoTransporte>.Fallo(DomainErrors.Accesos.ViajeRequerido);
            }

            var evaluacion = EvaluarAcceso(usuario, autorizacion, viaje, fechaHora, costoViaje);

            return CrearRegistroIntento(
                usuario.Id,
                viaje.Id,
                autorizacion?.Id,
                validadoPorUsuarioId,
                evaluacion.Resultado,
                evaluacion.MotivoRechazo,
                fechaHora);
        }

        public static Result<RegistroUsoTransporte> RegistrarAbordajePermitido(
            UsuarioTransporte? usuario,
            AutorizacionTransporte? autorizacion,
            Viaje? viaje,
            int validadoPorUsuarioId,
            DateTime fechaHora,
            decimal costoViaje = AutorizacionRules.CostoViajePredeterminado)
        {
            var validacion = ValidarAcceso(usuario, autorizacion, viaje, fechaHora, costoViaje);

            if (validacion.EsFallo)
            {
                return Result<RegistroUsoTransporte>.Fallo(validacion.Error!);
            }

            var consumo = AutorizacionRules.ConsumirAutorizacion(autorizacion, fechaHora, costoViaje);

            if (consumo.EsFallo)
            {
                return Result<RegistroUsoTransporte>.Fallo(consumo.Error!);
            }

            viaje!.CupoActual++;

            return CrearRegistroIntento(
                usuario!.Id,
                viaje.Id,
                autorizacion!.Id,
                validadoPorUsuarioId,
                ResultadoAcceso.Permitido,
                null,
                fechaHora);
        }

        private static ResultadoAcceso MapearResultado(DomainError error)
        {
            if (error == DomainErrors.Accesos.UsuarioInactivo)
            {
                return ResultadoAcceso.UsuarioInactivo;
            }

            if (error == DomainErrors.Accesos.ViajeNoDisponible)
            {
                return ResultadoAcceso.ViajeNoDisponible;
            }

            if (error == DomainErrors.Accesos.SinCupo)
            {
                return ResultadoAcceso.SinCupo;
            }

            if (error == DomainErrors.Accesos.AutorizacionRequerida)
            {
                return ResultadoAcceso.SinAutorizacion;
            }

            if (error == DomainErrors.Autorizaciones.TicketVencido)
            {
                return ResultadoAcceso.AutorizacionVencida;
            }

            if (error == DomainErrors.Autorizaciones.SaldoInsuficiente)
            {
                return ResultadoAcceso.SaldoInsuficiente;
            }

            if (error == DomainErrors.Autorizaciones.AutorizacionNoPerteneceAlUsuario ||
                error == DomainErrors.Autorizaciones.TarjetaInactiva ||
                error == DomainErrors.Autorizaciones.TipoNoSoportado)
            {
                return ResultadoAcceso.AutorizacionInvalida;
            }

            return ResultadoAcceso.Denegado;
        }
    }
}
