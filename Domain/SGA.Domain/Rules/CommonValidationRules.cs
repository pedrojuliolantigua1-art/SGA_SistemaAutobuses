using SGA.Domain.Error;

namespace SGA.Domain.Rules
{
    public static class CommonValidationRules
    {
        public static Result Requerido(string? valor, string campo)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? Result.Fallo(DomainErrors.General.CampoRequerido(campo))
                : Result.Ok();
        }

        public static Result IdValido(int id, string campo)
        {
            return id <= 0
                ? Result.Fallo(DomainErrors.General.IdInvalido(campo))
                : Result.Ok();
        }

        public static Result MontoPositivo(decimal monto, string campo)
        {
            return monto <= 0
                ? Result.Fallo(DomainErrors.General.MontoInvalido(campo))
                : Result.Ok();
        }

        public static Result EnteroPositivo(int valor, string campo)
        {
            return valor <= 0
                ? Result.Fallo(DomainErrors.General.IdInvalido(campo))
                : Result.Ok();
        }

        public static Result FechaDefinida(DateTime fecha, string campo)
        {
            return fecha == default
                ? Result.Fallo(DomainErrors.General.FechaInvalida(campo))
                : Result.Ok();
        }

        public static Result RangoFechasValido(DateTime desde, DateTime hasta, string contexto)
        {
            var fechasDefinidas = Combinar(
                FechaDefinida(desde, $"{contexto} desde"),
                FechaDefinida(hasta, $"{contexto} hasta"));

            if (fechasDefinidas.EsFallo)
            {
                return fechasDefinidas;
            }

            return hasta < desde
                ? Result.Fallo(DomainErrors.General.RangoFechaInvalido(contexto))
                : Result.Ok();
        }

        public static Result Combinar(params Result[] resultados)
        {
            foreach (var resultado in resultados)
            {
                if (resultado.EsFallo)
                {
                    return resultado;
                }
            }

            return Result.Ok();
        }
    }
}
