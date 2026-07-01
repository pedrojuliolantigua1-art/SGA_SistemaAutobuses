using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules.Auditoria
{
    public static class AuditoriaRules
    {
        public static Result<RegistroAuditoria> CrearRegistro(
            int usuarioTransporteId,
            string? accion,
            string? entidadAfectada,
            string? entidadId,
            string? detalle,
            DateTime fechaHora)
        {
            var validacion = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(usuarioTransporteId, "actor"),
                ValidationGeneral.Requerido(accion, "accion"),
                ValidationGeneral.Requerido(entidadAfectada, "entidad afectada"),
                ValidationGeneral.Requerido(entidadId, "id de entidad"),
                ValidationGeneral.Requerido(detalle, "detalle de auditoria"),
                ValidationGeneral.FechaDefinida(fechaHora, "auditoria"));

            if (validacion.EsFallo)
            {
                return Result<RegistroAuditoria>.Fallo(validacion.Error!);
            }

            return Result<RegistroAuditoria>.Ok(new RegistroAuditoria
            {
                UsuarioTransporteId = usuarioTransporteId,
                Accion = accion,
                EntidadAfectada = entidadAfectada,
                EntidadId = entidadId,
                Detalle = detalle,
                FechaHora = fechaHora
            });
        }

        public static Result PuedeModificarRegistro(RegistroAuditoria? registro)
        {
            return registro is null
                ? Result.Fallo(DomainErrors.General.CampoRequerido("registro de auditoria"))
                : Result.Fallo(DomainErrors.Auditoria.RegistroInmutable);
        }

        public static Result PuedeEliminarRegistro(RegistroAuditoria? registro)
        {
            return PuedeModificarRegistro(registro);
        }
    }
}
