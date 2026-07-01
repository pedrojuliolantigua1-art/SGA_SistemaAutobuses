using SGA.Domain.Entities.Transporte;
using SGA.Domain.Error;
using SGA.Domain.Validation;

namespace SGA.Domain.Rules
{
    public static class RutaRules
    {
        // Aquí se utiliza Parada para validar las reglas de una ruta,
        // ya que una ruta debe tener al menos 2 paradas y cada una
        // debe tener un orden mayor que 0.
        public static Result Validar(Ruta? ruta, IEnumerable<Parada>? paradas = null)
        {
            if (ruta is null)
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido("ruta"));
            }

            var validacion = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(ruta.Id, "ruta"),
                ValidationGeneral.RequeridoConLongitud(ruta.Nombre, "nombre de la ruta", min: 3, max: 100));

            if (validacion.EsFallo)
            {
                return validacion;
            }

            if (!ruta.Activa)
            {
                return Result.Fallo(DomainErrors.CatalogoTransporte.RutaInactiva);
            }

            if (paradas is null)
            {
                return Result.Ok();
            }

            int cantidadParadas = 0;

            foreach (Parada parada in paradas)
            {
                cantidadParadas++;

                if (parada.Orden <= 0)
                {
                    return Result.Fallo(DomainErrors.General.IdInvalido("orden de parada"));
                }
            }

            if (cantidadParadas < 2)
            {
                return Result.Fallo(DomainErrors.CatalogoTransporte.RutaSinParadasSuficientes);
            }

            return Result.Ok();
        }
    }
}