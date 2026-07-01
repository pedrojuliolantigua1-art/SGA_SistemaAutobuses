namespace SGA.Domain.Error
{
    public static partial class DomainErrors
    {
        public static class Autorizaciones
        {
            public static readonly Error PagoRequerido =
                new("Autorizacion.PagoRequerido", "Toda autorizacion debe estar asociada a un pago registrado.");

            public static readonly Error PagoNoVerificable =
                new("Autorizacion.PagoNoVerificable", "El pago no tiene los datos necesarios para ser verificable.");

            public static readonly Error PagoNoPerteneceAlUsuario =
                new("Autorizacion.PagoNoPerteneceAlUsuario", "El pago no pertenece al usuario indicado.");

            public static readonly Error AutorizacionNoPerteneceAlUsuario =
                new("Autorizacion.AutorizacionNoPerteneceAlUsuario", "La autorizacion no pertenece al usuario indicado.");

            public static readonly Error TicketActivoExistente =
                new("Autorizacion.TicketActivoExistente", "El usuario ya tiene un ticket mensual activo.");

            public static readonly Error TicketVencido =
                new("Autorizacion.TicketVencido", "El ticket mensual no esta vigente.");

            public static readonly Error SaldoInsuficiente =
                new("Autorizacion.SaldoInsuficiente", "La tarjeta no tiene saldo suficiente.");

            public static readonly Error TarjetaInactiva =
                new("Autorizacion.TarjetaInactiva", "La tarjeta recargable no esta activa.");

            public static readonly Error TipoNoSoportado =
                new("Autorizacion.TipoNoSoportado", "El tipo de autorizacion no esta soportado para acceso.");

            public static readonly Error FechaVencimientoEnElPasado =
                new("FechaVencimientoEnElPasado", "La fecha de vencimiento ya paso");

            public static readonly Error AutorizacionYaAnulada =
                new("AutorizacionYaAnulada", "Su autorizacion ya fue anulada anteriormente");
        }
    }
}
