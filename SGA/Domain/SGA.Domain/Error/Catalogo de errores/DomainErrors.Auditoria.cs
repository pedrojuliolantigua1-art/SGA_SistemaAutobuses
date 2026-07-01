namespace SGA.Domain.Error
{
    public static partial class DomainErrors
    {
        public static class Auditoria
        {
            public static readonly Error RegistroInmutable =
                new("Auditoria.RegistroInmutable", "Los registros de auditoria no pueden modificarse ni eliminarse por usuarios operativos.");
        }
    }
}
