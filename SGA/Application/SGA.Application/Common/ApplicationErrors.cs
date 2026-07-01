using SGA.Domain.Error;

namespace SGA.Application.Common
{
    public static class ApplicationErrors
    {
        public static Error NoEncontrado(string entidad) =>
            new("Aplicacion.NoEncontrado", $"No se encontro {entidad}.");

        public static Error OperacionInvalida(string mensaje) =>
            new("Aplicacion.OperacionInvalida", mensaje);
    }
}
