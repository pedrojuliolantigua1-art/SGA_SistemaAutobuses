using SGA.Application.DTOs.Pagos;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IPagoService
    {
        Task<Result<PagoDto>> RegistrarAsync(RegistrarPagoDto dto);
        Task<Result<IReadOnlyList<PagoDto>>> ListarPorUsuarioAsync(int usuarioId);
        Task<Result<PagoDto>> ObtenerPorIdAsync(int pagoId);
        Task<Result<IReadOnlyList<PagoDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta);
    }
}