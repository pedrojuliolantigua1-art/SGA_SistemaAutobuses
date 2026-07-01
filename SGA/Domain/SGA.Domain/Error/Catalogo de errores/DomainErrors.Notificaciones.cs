namespace SGA.Domain.Error
{
    public static partial class DomainErrors
    {
        public static class Notificaciones
        {
            public static readonly Error EventoNoNotificable =
                new("Notificacion.EventoNoNotificable", "El evento indicado no requiere notificacion.");
        }
    }
}
