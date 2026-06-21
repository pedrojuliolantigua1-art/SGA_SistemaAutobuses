namespace SGA.Domain.Error
{
    public static partial class DomainErrors
    {
        public static class General
        {
            public static Error CampoRequerido(string campo) =>
                new("General.CampoRequerido", $"El campo {campo} es obligatorio.");

            public static Error IdInvalido(string campo) =>
                new("General.IdInvalido", $"El identificador de {campo} debe ser mayor que cero.");

            public static Error MontoInvalido(string campo) =>
                new("General.MontoInvalido", $"El monto de {campo} debe ser mayor que cero.");

            public static Error FechaInvalida(string campo) =>
                new("General.FechaInvalida", $"La fecha de {campo} debe estar definida.");

            public static Error RangoFechaInvalido(string contexto) =>
                new("General.RangoFechaInvalido", $"El rango de fechas de {contexto} no es valido.");
        }
    }
}
