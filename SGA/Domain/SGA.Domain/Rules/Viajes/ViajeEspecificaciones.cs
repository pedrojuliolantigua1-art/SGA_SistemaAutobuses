using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Models.Viajes;

namespace SGA.Domain.Rules
{
    // Aqui solamente las especificaciones de negocio que no son validaciones de datos como tal,
    // sino reglas de negocio que determinan si un viaje yo puedo iniciarlo, finalizarloy asi
    public static class ViajeEspecificaciones
    {
        public static bool PuedeIniciar(Viaje viaje) =>
            viaje.Estado == EstadoViaje.Programado ||
            viaje.Estado == EstadoViaje.Retrasado && viaje.HoraInicioReal is null;

        public static bool PuedeFinalizar(Viaje viaje) =>
            viaje.Estado == EstadoViaje.EnCurso ||
            viaje.Estado == EstadoViaje.Retrasado && viaje.HoraInicioReal is not null;

        public static bool PuedeCancelar(Viaje viaje) =>
            viaje.Estado == EstadoViaje.Programado ||
            viaje.Estado == EstadoViaje.Retrasado && viaje.HoraInicioReal is null;

        public static bool PuedeMarcarRetrasado(Viaje viaje) =>
            viaje.Estado is EstadoViaje.Programado or EstadoViaje.EnCurso;

        public static bool TieneCupoDisponible(Viaje viaje) =>
            viaje.CupoActual < viaje.CapacidadMaxima;

        public static bool EstaEnEjecucion(Viaje viaje) =>
            viaje.Estado == EstadoViaje.EnCurso ||
            viaje.Estado == EstadoViaje.Retrasado && viaje.HoraInicioReal is not null;

        public static bool EsActivoParaAsignacion(Viaje viaje) =>
            viaje.Estado is EstadoViaje.Programado or EstadoViaje.EnCurso or EstadoViaje.Retrasado;

        public static bool PuedeTransicionar(Viaje viaje, EstadoViaje nuevoEstado)
        {
            return (viaje.Estado, nuevoEstado) switch
            {
                (EstadoViaje.Programado, EstadoViaje.EnCurso) => true,
                (EstadoViaje.Programado, EstadoViaje.Cancelado) => true,
                (EstadoViaje.Programado, EstadoViaje.Retrasado) => true,
                (EstadoViaje.Retrasado, EstadoViaje.EnCurso) when viaje.HoraInicioReal is null => true,
                (EstadoViaje.Retrasado, EstadoViaje.Cancelado) when viaje.HoraInicioReal is null => true,
                (EstadoViaje.EnCurso, EstadoViaje.Finalizado) => true,
                (EstadoViaje.EnCurso, EstadoViaje.Retrasado) => true,
                (EstadoViaje.Retrasado, EstadoViaje.Finalizado) when viaje.HoraInicioReal is not null => true,
                _ => false
            };
        }
    }
}
