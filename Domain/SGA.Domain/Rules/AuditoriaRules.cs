using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Error;

namespace SGA.Domain.Rules
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
            var validacion = CommonValidationRules.Combinar(
                CommonValidationRules.IdValido(usuarioTransporteId, "actor"),
                CommonValidationRules.Requerido(accion, "accion"),
                CommonValidationRules.Requerido(entidadAfectada, "entidad afectada"),
                CommonValidationRules.Requerido(entidadId, "id de entidad"),
                CommonValidationRules.Requerido(detalle, "detalle de auditoria"),
                CommonValidationRules.FechaDefinida(fechaHora, "auditoria"));

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
