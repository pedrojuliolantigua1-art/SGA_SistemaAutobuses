namespace SGA.Application.DTOs.Auditoria
{
    public sealed record AuditoriaDto(
        int Id, int UsuarioTransporteId, string? Accion, string? EntidadAfectada,
        string? EntidadId, string? Detalle, DateTime FechaHora);
}
