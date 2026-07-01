using SGA.Domain.Entities.Accesos;
using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Error;
using SGA.Domain.Validation;
using DomainError = SGA.Domain.Error.Error;

namespace SGA.Domain.Rules.Accesos
{
    public static class AccesoRules
    {
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

            if (!UsuarioBaseRules.EstaActivo(usuario.Estado))
            {
                return Result.Fallo(DomainErrors.Accesos.UsuarioInactivo);
            }

            if (viaje is null)
            {
                return Result.Fallo(DomainErrors.Accesos.ViajeRequerido);
            }

            if (!ViajeEspecificaciones.EstaEnEjecucion(viaje))
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

            var autorizacionValida = AutorizacionRules.ValidarAutorizacionParaAcceso(autorizacion, fechaHora, costoViaje);

            if (autorizacionValida.EsFallo)
            {
                return autorizacionValida;
            }

            return ViajeEspecificaciones.TieneCupoDisponible(viaje)
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
            var validacion = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(usuarioTransporteId, "usuario"),
                ValidationGeneral.IdValido(viajeId, "viaje"),
                ValidationGeneral.IdValido(validadoPorUsuarioId, "usuario validador"),
                ValidationGeneral.FechaDefinida(fechaHora, "intento de acceso"));

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

            return AutorizacionRules.ResultadoAccesoDesdeError(error);
        }
    }
}