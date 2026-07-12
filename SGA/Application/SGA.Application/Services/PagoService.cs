using SGA.Application.Common;
using SGA.Application.DTOs.Pagos;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Error;
using SGA.Domain.Models.Pagos;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Validation;

namespace SGA.Application.Services
{
    public sealed class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public PagoService(IPagoRepository pagoRepository, IUsuarioRepository usuarioRepository)
        {
            _pagoRepository = pagoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Result<PagoDto>> RegistrarAsync(RegistrarPagoDto dto)
        {
            var validacion = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(dto.UsuarioTransporteId, "usuario"),
                ValidationGeneral.MontoPositivo(dto.Monto, "pago"),
                ValidationGeneral.Requerido(dto.TipoPago, "tipo de pago"),
                ValidationGeneral.Requerido(dto.NumeroComprobante, "numero de comprobante"),
                ValidationGeneral.FechaDefinida(dto.FechaHora, "pago"));

            if (validacion.EsFallo)
            {
                return Result<PagoDto>.Fallo(validacion.Error!);
            }

            var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioTransporteId);
            if (usuario is null) return Result<PagoDto>.Fallo(ApplicationErrors.NoEncontrado("el usuario"));

            var entity = new PagoTransporte
            {
                UsuarioTransporteId = dto.UsuarioTransporteId,
                Monto = dto.Monto,
                TipoPago = dto.TipoPago,
                Estado = SGA.Domain.Enum.EstadoPago.Registrado,
                NumeroComprobante = dto.NumeroComprobante,
                FechaHora = dto.FechaHora,
                RegistradoPorUsuarioId = dto.RegistradoPorUsuarioId,
                FechaCreacion = DateTime.UtcNow,
                CreadoPor = dto.CreadoPor
            };

            await _pagoRepository.AddAsync(entity);
            return Result<PagoDto>.Ok(new(entity.Id, entity.UsuarioTransporteId, entity.AutorizacionTransporteId, entity.Monto, entity.TipoPago, entity.Estado, entity.NumeroComprobante, entity.FechaHora, entity.RegistradoPorUsuarioId));
        }

        public async Task<Result<IReadOnlyList<PagoDto>>> ListarPorUsuarioAsync(int usuarioId)
        {
            var validacion = ValidationGeneral.IdValido(usuarioId, "usuario");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<PagoDto>>.Fallo(validacion.Error!);
            }

            var pagos = await _pagoRepository.GetByUsuario(usuarioId);
            return Result<IReadOnlyList<PagoDto>>.Ok(pagos.Select(Mapear).ToList());
        }

        public async Task<Result<PagoDto>> ObtenerPorIdAsync(int pagoId)
        {
            var validacion = ValidationGeneral.IdValido(pagoId, "pago");
            if (validacion.EsFallo)
                return Result<PagoDto>.Fallo(validacion.Error!);

            var pago = await _pagoRepository.GetByIdAsync(pagoId);
            return pago is null
                ? Result<PagoDto>.Fallo(ApplicationErrors.NoEncontrado("el pago"))
                : Result<PagoDto>.Ok(Mapear(pago));
        }

        public async Task<Result<IReadOnlyList<PagoDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta)
        {
            var validacion = ValidationGeneral.RangoFechasValido(desde, hasta, "pagos");
            if (validacion.EsFallo)
                return Result<IReadOnlyList<PagoDto>>.Fallo(validacion.Error!);

            var pagos = await _pagoRepository.GetByPeriodo(desde, hasta);
            return Result<IReadOnlyList<PagoDto>>.Ok(pagos.Select(Mapear).ToList());
        }

        private static PagoDto Mapear(PagoModel p) =>
            new(p.Id, p.UsuarioTransporteId, p.AutorizacionTransporteId, p.Monto, p.TipoPago, p.Estado, p.NumeroComprobante, p.FechaHora, p.RegistradoPorUsuarioId);
    }
}
