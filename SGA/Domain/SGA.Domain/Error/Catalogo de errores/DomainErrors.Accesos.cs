namespace SGA.Domain.Error
{
    public static partial class DomainErrors
    {
        public static class Accesos
        {
            public static readonly Error UsuarioRequerido =
                new("Acceso.UsuarioRequerido", "El usuario es obligatorio para validar el acceso.");

            public static readonly Error UsuarioInactivo =
                new("Acceso.UsuarioInactivo", "El usuario no esta activo.");

            public static readonly Error ViajeRequerido =
                new("Acceso.ViajeRequerido", "El viaje es obligatorio para validar el acceso.");

            public static readonly Error ViajeNoDisponible =
                new("Acceso.ViajeNoDisponible", "El viaje no esta en curso para permitir abordaje.");

            public static readonly Error AutorizacionRequerida =
                new("Acceso.AutorizacionRequerida", "El usuario no tiene autorizacion de transporte.");

            public static readonly Error SinCupo =
                new("Acceso.SinCupo", "El autobus ya alcanzo su capacidad maxima.");
        }
    }
}
