namespace SGA.Application.DTOs.Notificaciones
{
    public sealed record NotificacionDto(
        int Id, int UsuarioTransporteId, string Tipo, string Titulo,
        string Mensaje, DateTime FechaHora, bool Leida);

    public sealed record CrearNotificacionDto(
        int UsuarioTransporteId, string Tipo, string Titulo,
        string Mensaje, DateTime FechaHora, string? CreadoPor);
}
