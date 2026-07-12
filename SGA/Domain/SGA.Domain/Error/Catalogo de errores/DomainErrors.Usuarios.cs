namespace SGA.Domain.Error
{
    public static partial class DomainErrors
    {
        public static class Usuarios
        {
            public static readonly Error CorreoDuplicado =
                new("Usuarios.CorreoDuplicado", "Ya existe un usuario registrado con ese correo.");

            public static readonly Error CodigoEmpleadoDuplicado =
                new("Usuarios.CodigoEmpleadoDuplicado", "Ya existe un empleado registrado con ese codigo.");
        }
    }
}
