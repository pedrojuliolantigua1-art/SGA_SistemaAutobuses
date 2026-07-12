namespace SGA.Domain.Error
{
    public static partial class DomainErrors
    {
        public static class CatalogoTransporte
        {
            public static readonly Error RutaInactiva =
                new("CatalogoTransporte.RutaInactiva", "La ruta debe estar activa.");

            public static readonly Error RutaSinParadasSuficientes =
                new("CatalogoTransporte.RutaSinParadasSuficientes", "La ruta debe tener al menos dos paradas.");

            public static readonly Error HorarioInactivo =
                new("CatalogoTransporte.HorarioInactivo", "El horario debe estar activo.");

            public static readonly Error HorarioNoPerteneceRuta =
                new("CatalogoTransporte.HorarioNoPerteneceRuta", "El horario no pertenece a la ruta indicada.");

            public static readonly Error HorarioInvalido =
                new("CatalogoTransporte.HorarioInvalido", "La hora de llegada estimada debe ser distinta a la hora de salida.");

            public static readonly Error AutobusNoDisponible =
                new("CatalogoTransporte.AutobusNoDisponible", "El autobus no esta disponible.");

            public static readonly Error ConductorNoDisponible =
                new("CatalogoTransporte.ConductorNoDisponible", "El conductor no esta disponible para asignacion.");

            public static readonly Error OrdenParadaDuplicado =
                new("CatalogoTransporte.OrdenParadaDuplicado", "No puede haber dos paradas con el mismo orden en la misma ruta.");

            public static readonly Error PlacaDuplicada =
                new("CatalogoTransporte.PlacaDuplicada", "Ya existe un autobus registrado con esa placa.");

            public static readonly Error LicenciaDuplicada =
                new("CatalogoTransporte.LicenciaDuplicada", "Ya existe un conductor registrado con ese numero de licencia.");

            public static readonly Error LicenciaVencida =
                new("CatalogoTransporte.LicenciaVencida", "La licencia del conductor esta vencida.");
        }
    }
}
