namespace SGA.Domain.Error
{
    public static partial class DomainErrors
    {
        public static class Viajes
        {
            public static readonly Error ViajeRequerido =
                new("Viaje.ViajeRequerido", "El viaje es obligatorio.");

            public static readonly Error PlanificacionInvalida =
                new("Viaje.PlanificacionInvalida", "El viaje debe tener ruta, horario, autobus y conductor asignados.");

            public static readonly Error CupoInvalido =
                new("Viaje.CupoInvalido", "El cupo del viaje no puede ser negativo ni exceder la capacidad maxima.");

            public static readonly Error CapacidadInvalida =
                new("Viaje.CapacidadInvalida", "La capacidad maxima del viaje debe ser mayor que cero.");

            public static readonly Error EstadoInvalido =
                new("Viaje.EstadoInvalido", "La transicion de estado del viaje no es valida.");

            public static readonly Error ConductorNoAsignado =
                new("Viaje.ConductorNoAsignado", "El conductor indicado no esta asignado a este viaje.");

            public static readonly Error ConflictoAutobus =
                new("Viaje.ConflictoAutobus", "El autobus ya esta asignado a otro viaje activo en el mismo horario.");

            public static readonly Error ConflictoConductor =
                new("Viaje.ConflictoConductor", "El conductor ya esta asignado a otro viaje activo en el mismo horario.");

            public static readonly Error MotivoCancelacionRequerido =
                new("Viaje.MotivoCancelacionRequerido", "El motivo de cancelacion es obligatorio.");

            public static readonly Error IncidenciaDescripcionRequerida =
                new("Viaje.IncidenciaDescripcionRequerida", "La descripcion de la incidencia es obligatoria.");
        }
    }
}
