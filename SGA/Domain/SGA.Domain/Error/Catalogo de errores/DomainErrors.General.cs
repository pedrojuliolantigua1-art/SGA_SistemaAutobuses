namespace SGA.Domain.Error
{
    public static partial class DomainErrors
    {
        public static class General
        {
            public static Error CampoRequerido(string campo)
            {
                return new Error("General.CampoRequerido",$"El campo {campo} es obligatorio.");
            }

            public static Error IdInvalido(string campo)
            {
                return new Error("General.IdInvalido",$"El identificador de {campo} debe ser mayor que cero.");
            }

            public static Error MontoInvalido(string campo)
            {
                return new Error("General.MontoInvalido",$"El monto de {campo} debe ser mayor que cero.");
            }

            public static Error FechaInvalida(string campo)
            {
                return new Error( "General.FechaInvalida",$"La fecha de {campo} debe estar definida.");
            }

            public static Error RangoFechaInvalido(string contexto)
            {
                return new Error("General.RangoFechaInvalido",$"El rango de fechas de {contexto} no es valido.");
            }

            public static Error LongitudInvalida(string campo, int min, int max)
            {
                if (max == int.MaxValue)
                {
                    return new Error("General.LongitudInvalida",$"El campo {campo} debe tener al menos {min} caracteres.");
                }
                else
                {
                    return new Error("General.LongitudInvalida",$"El campo {campo} debe tener entre {min} y {max} caracteres.");
                }
            }

            public static Error FormatoInvalido(string campo, string formatoEsperado)
            {
                return new Error("General.FormatoInvalido", $"El campo {campo} no tiene el formato esperado: {formatoEsperado}.");
            }
        }
    }
}