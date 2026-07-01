using SGA.Domain.Error;
using System.Text.RegularExpressions;

namespace SGA.Domain.Validation
{
    public static class ValidationGeneral
    {
        public static Result Requerido(string? valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido(campo));
            }
            else
            {
                return Result.Ok();
            }
        }

        public static Result IdValido(int id, string campo)
        {
            if (id <= 0)
            {
                return Result.Fallo(DomainErrors.General.IdInvalido(campo));
            }
            else
            {
                return Result.Ok();
            }
        }

        public static Result MontoPositivo(decimal monto, string campo)
        {
            if (monto <= 0)
            {
                return Result.Fallo(DomainErrors.General.MontoInvalido(campo));
            }
            else
            {
                return Result.Ok();
            }
        }

        public static Result EnteroPositivo(int valor, string campo)
        {
            if (valor <= 0)
            {
                return Result.Fallo(DomainErrors.General.IdInvalido(campo));
            }
            else
            {
                return Result.Ok();
            }
        }

        public static Result FechaDefinida(DateTime fecha, string campo)
        {
            if (fecha == default)
            {
                return Result.Fallo(DomainErrors.General.FechaInvalida(campo));
            }
            else
            {
                return Result.Ok();
            }
        }

        public static Result RangoFechasValido(DateTime desde, DateTime hasta, string contexto)
        {
            Result fechasDefinidas = Combinar(
                FechaDefinida(desde, $"{contexto} desde"),
                FechaDefinida(hasta, $"{contexto} hasta"));

            if (fechasDefinidas.EsFallo)
            {
                return fechasDefinidas;
            }

            if (hasta < desde)
            {
                return Result.Fallo(DomainErrors.General.RangoFechaInvalido(contexto));
            }
            else
            {
                return Result.Ok();
            }
        }

        public static Result LongitudValida(string? valor, string campo, int min = 0, int max = int.MaxValue)
        {
            int longitud = valor?.Trim().Length ?? 0;

            if (longitud < min || longitud > max)
            {
                return Result.Fallo(DomainErrors.General.LongitudInvalida(campo, min, max));
            }
            else
            {
                return Result.Ok();
            }
        }

        public static Result RequeridoConLongitud(string? valor, string campo, int min = 1, int max = int.MaxValue)
        {
            return Combinar(
                Requerido(valor, campo),
                LongitudValida(valor, campo, min, max));
        }

        public static Result FormatoValido(string? valor, string campo, string patron, string formatoEsperado)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return Result.Fallo(DomainErrors.General.CampoRequerido(campo));
            }

            if (Regex.IsMatch(valor.Trim(), patron))
            {
                return Result.Ok();
            }
            else
            {
                return Result.Fallo(DomainErrors.General.FormatoInvalido(campo, formatoEsperado));
            }
        }

        public static Result Combinar(params Result[] resultados)
        {
            foreach (Result resultado in resultados)
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