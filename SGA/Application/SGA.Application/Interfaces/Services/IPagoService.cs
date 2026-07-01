using SGA.Application.DTOs.Pagos;
using SGA.Domain.Error;

namespace SGA.Application.Interfaces.Services
{
    public interface IPagoService
    {
        Task<Result<PagoDto>> RegistrarAsync(RegistrarPagoDto dto);
        Task<Result<IReadOnlyList<PagoDto>>> ListarPorUsuarioAsync(int usuarioId);
    }
}