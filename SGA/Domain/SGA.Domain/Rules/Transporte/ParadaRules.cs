using SGA.Domain.Entities.Transporte;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules
{
    public static class ParadaRules
    {
        public static Result Validar(Parada? parada)
        {
            if (parada is null)
                return Result.Fallo(DomainErrors.General.CampoRequerido("parada"));

            return ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(parada.RutaId, "ruta de la parada"),
                ValidationGeneral.RequeridoConLongitud(parada.Nombre, "nombre de la parada", min: 3, max: 100),
                ValidationGeneral.EnteroPositivo(parada.Orden, "orden de la parada"));
        }


        // Valida que no haya dos paradas con el mismo Orden dentro de la misma ruta.
        public static Result ValidarOrdenUnico(IEnumerable<Parada> paradasDeLaRuta)
        {
            var duplicados = paradasDeLaRuta
                .GroupBy(p => p.Orden)
                .Any(g => g.Count() > 1);

            return duplicados
                ? Result.Fallo(DomainErrors.CatalogoTransporte.OrdenParadaDuplicado) 
                : Result.Ok();
        }
    }
}