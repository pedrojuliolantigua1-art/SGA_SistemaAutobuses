namespace SGA.Application.DTOs.Common
{
    public sealed record RestaurarDto(
        string? RestauradoPor,
        string? Motivo = null);
}
