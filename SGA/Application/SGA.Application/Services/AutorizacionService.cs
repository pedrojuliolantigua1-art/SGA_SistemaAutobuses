using SGA.Application.Common;
using SGA.Application.DTOs.Autorizaciones;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Error;
using SGA.Domain.Models.Autorizaciones;
using SGA.Domain.Models.Pagos;
using SGA.Domain.Models.Usuarios;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Rules;
using SGA.Domain.Validation;

namespace SGA.Application.Services
{
    public sealed class AutorizacionService : IAutorizacionService
    {
        private readonly IAutorizacionRepository _autorizacionRepository;
        private readonly IPagoRepository _pagoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public AutorizacionService(
            IAutorizacionRepository autorizacionRepository,
            IPagoRepository pagoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _autorizacionRepository = autorizacionRepository;
            _pagoRepository = pagoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Result<AutorizacionDto>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            var validacion = ValidationGeneral.IdValido(usuarioId, "usuario");

            if (validacion.EsFallo)
            {
                return Result<AutorizacionDto>.Fallo(validacion.Error!);
            }

            var autorizacion = await ObtenerAutorizacionActivaAsync(usuarioId);

            return autorizacion is null
                ? Result<AutorizacionDto>.Fallo(ApplicationErrors.NoEncontrado("la autorizacion del usuario"))
                : Result<AutorizacionDto>.Ok(MapearAutorizacion(autorizacion));
        }

        public async Task<Result<IReadOnlyList<AutorizacionDto>>> ListarVigentesAsync()
        {
            var autorizaciones = await _autorizacionRepository.GetVigentes();
            return Result<IReadOnlyList<AutorizacionDto>>.Ok(autorizaciones.Select(MapearAutorizacion).ToList());
        }

        public async Task<Result<IReadOnlyList<AutorizacionDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta)
        {
            var validacion = ValidationGeneral.RangoFechasValido(desde, hasta, "autorizaciones");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<AutorizacionDto>>.Fallo(validacion.Error!);
            }

            var autorizaciones = await _autorizacionRepository.GetbyPeriodo(desde, hasta);
            return Result<IReadOnlyList<AutorizacionDto>>.Ok(autorizaciones.Select(MapearAutorizacion).ToList());
        }

        public async Task<Result<AutorizacionDto>> EmitirTicketMensualAsync(CrearTicketMensualDto dto)
        {
            var usuarioValido = await ValidarUsuarioActivoAsync(dto.UsuarioTransporteId);

            if (usuarioValido.EsFallo)
            {
                return Result<AutorizacionDto>.Fallo(usuarioValido.Error!);
            }

            var autorizacionActual = await ObtenerAutorizacionActivaAsync(dto.UsuarioTransporteId);

            if (autorizacionActual is not null)
            {
                return Result<AutorizacionDto>.Fallo(
                    ApplicationErrors.OperacionInvalida("El usuario ya tiene una autorizacion activa."));
            }

            var pago = ConvertirPago(await _pagoRepository.GetPagoSinAutorizacion(dto.UsuarioTransporteId));
            var ticketCreado = AutorizacionRules.CrearTicketMensual(pago, dto.UsuarioTransporteId, dto.FechaInicio);

            if (ticketCreado.EsFallo)
            {
                return Result<AutorizacionDto>.Fallo(ticketCreado.Error!);
            }

            var ticket = ticketCreado.Valor!;
            ticket.FechaCreacion = DateTime.UtcNow;
            ticket.CreadoPor = dto.CreadoPor;

            await _autorizacionRepository.AddAsync(ticket);
            await MarcarPagoComoAplicadoAsync(pago!);

            return Result<AutorizacionDto>.Ok(MapearAutorizacion(ticket));
        }

        public async Task<Result<AutorizacionDto>> EmitirTarjetaRecargableAsync(CrearTarjetaRecargableDto dto)
        {
            var usuarioValido = await ValidarUsuarioActivoAsync(dto.UsuarioTransporteId);

            if (usuarioValido.EsFallo)
            {
                return Result<AutorizacionDto>.Fallo(usuarioValido.Error!);
            }

            var autorizacionActual = await ObtenerAutorizacionActivaAsync(dto.UsuarioTransporteId);

            if (autorizacionActual is not null)
            {
                return Result<AutorizacionDto>.Fallo(
                    ApplicationErrors.OperacionInvalida("El usuario ya tiene una autorizacion activa."));
            }

            var pago = ConvertirPago(await _pagoRepository.GetPagoSinAutorizacion(dto.UsuarioTransporteId));
            var tarjetaCreada = AutorizacionRules.CrearTarjetaRecargable(
                pago,
                dto.UsuarioTransporteId,
                dto.SaldoInicial,
                dto.NumeroTarjeta);

            if (tarjetaCreada.EsFallo)
            {
                return Result<AutorizacionDto>.Fallo(tarjetaCreada.Error!);
            }

            var tarjeta = tarjetaCreada.Valor!;
            tarjeta.FechaCreacion = DateTime.UtcNow;
            tarjeta.CreadoPor = dto.CreadoPor;

            await _autorizacionRepository.AddAsync(tarjeta);
            await MarcarPagoComoAplicadoAsync(pago!);

            return Result<AutorizacionDto>.Ok(MapearAutorizacion(tarjeta));
        }

        public async Task<Result<AutorizacionDto>> EmitirPermisoAsync(CrearPermisoTransporteDto dto)
        {
            var usuarioValido = await ValidarUsuarioActivoAsync(dto.UsuarioTransporteId);

            if (usuarioValido.EsFallo)
            {
                return Result<AutorizacionDto>.Fallo(usuarioValido.Error!);
            }

            var autorizacionActual = await ObtenerAutorizacionActivaAsync(dto.UsuarioTransporteId);

            if (autorizacionActual is not null)
            {
                return Result<AutorizacionDto>.Fallo(
                    ApplicationErrors.OperacionInvalida("El usuario ya tiene una autorizacion activa."));
            }

            var permisoCreado = AutorizacionRules.CrearPermiso(
                dto.UsuarioTransporteId,
                dto.CondicionInstitucional,
                dto.FechaVencimiento);

            if (permisoCreado.EsFallo)
            {
                return Result<AutorizacionDto>.Fallo(permisoCreado.Error!);
            }

            var permiso = permisoCreado.Valor!;
            permiso.FechaCreacion = DateTime.UtcNow;
            permiso.CreadoPor = dto.CreadoPor;

            await _autorizacionRepository.AddAsync(permiso);
            return Result<AutorizacionDto>.Ok(MapearAutorizacion(permiso));
        }

        public async Task<Result> AnularAsync(int autorizacionId)
        {
            var validacion = ValidationGeneral.IdValido(autorizacionId, "autorizacion");

            if (validacion.EsFallo)
            {
                return Result.Fallo(validacion.Error!);
            }

            var autorizacionModel = await _autorizacionRepository.GetByIdAsync(autorizacionId);

            if (autorizacionModel is null)
            {
                return Result.Fallo(ApplicationErrors.NoEncontrado("la autorizacion"));
            }

            var autorizacion = ConvertirAutorizacion(autorizacionModel);
            var anulacion = AutorizacionRules.Anular(autorizacion);

            if (anulacion.EsFallo)
            {
                return anulacion;
            }

            autorizacion.FechaModificacion = DateTime.UtcNow;
            await _autorizacionRepository.UpdateAsync(autorizacion);
            return Result.Ok();
        }

        private async Task<Result<UsuarioModel>> ValidarUsuarioActivoAsync(int usuarioId)
        {
            var validacion = ValidationGeneral.IdValido(usuarioId, "usuario");

            if (validacion.EsFallo)
            {
                return Result<UsuarioModel>.Fallo(validacion.Error!);
            }

            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);

            if (usuario is null)
            {
                return Result<UsuarioModel>.Fallo(ApplicationErrors.NoEncontrado("el usuario"));
            }

            return UsuarioBaseRules.EstaActivo(usuario.Estado)
                ? Result<UsuarioModel>.Ok(usuario)
                : Result<UsuarioModel>.Fallo(DomainErrors.Accesos.UsuarioInactivo);
        }

        private async Task<AutorizacionModel?> ObtenerAutorizacionActivaAsync(int usuarioId)
        {
            try
            {
                return await _autorizacionRepository.GetbyUsuario(usuarioId);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private async Task MarcarPagoComoAplicadoAsync(PagoTransporte pago)
        {
            pago.Estado = "Aplicado";
            await _pagoRepository.UpdateAsync(pago);
        }

        private static PagoTransporte? ConvertirPago(PagoModel? pago)
        {
            if (pago is null)
            {
                return null;
            }

            return new PagoTransporte
            {
                Id = pago.Id,
                UsuarioTransporteId = pago.UsuarioTransporteId,
                AutorizacionTransporteId = pago.AutorizacionTransporteId,
                Monto = pago.Monto,
                TipoPago = pago.TipoPago,
                Estado = pago.Estado,
                NumeroComprobante = pago.NumeroComprobante,
                FechaHora = pago.FechaHora,
                RegistradoPorUsuarioId = pago.RegistradoPorUsuarioId
            };
        }

        private static AutorizacionTransporte ConvertirAutorizacion(AutorizacionModel model)
        {
            AutorizacionTransporte autorizacion = model switch
            {
                TicketMensualModel ticket => new TicketMensual
                {
                    FechaInicio = ticket.FechaInicio,
                    FechaFin = ticket.FechaFin
                },
                TarjetaRecargableModel tarjeta => new TarjetaRecargable
                {
                    NumeroTarjeta = tarjeta.NumeroTarjeta,
                    SaldoDisponible = tarjeta.SaldoDisponible
                },
                PermisoTransporteModel permiso => new PermisoTransporte
                {
                    CondicionInstitucional = permiso.CondicionInstitucional,
                    FechaVencimiento = permiso.FechaVencimiento
                },
                _ => throw new InvalidOperationException($"TipoAutorizacion desconocido: {model.GetType().Name}")
            };

            autorizacion.Id = model.Id;
            autorizacion.UsuarioTransporteId = model.UsuarioTransporteId;
            autorizacion.FechaEmision = model.FechaEmision;
            autorizacion.Estado = model.Estado;
            return autorizacion;
        }

        private static AutorizacionDto MapearAutorizacion(AutorizacionModel autorizacion)
        {
            return autorizacion switch
            {
                TicketMensualModel ticket => new(
                    ticket.Id,
                    ticket.UsuarioTransporteId,
                    nameof(TicketMensual),
                    ticket.FechaEmision,
                    ticket.Estado,
                    ticket.FechaInicio,
                    ticket.FechaFin,
                    null,
                    null,
                    null,
                    null),

                TarjetaRecargableModel tarjeta => new(
                    tarjeta.Id,
                    tarjeta.UsuarioTransporteId,
                    nameof(TarjetaRecargable),
                    tarjeta.FechaEmision,
                    tarjeta.Estado,
                    null,
                    null,
                    tarjeta.NumeroTarjeta,
                    tarjeta.SaldoDisponible,
                    null,
                    null),

                PermisoTransporteModel permiso => new(
                    permiso.Id,
                    permiso.UsuarioTransporteId,
                    nameof(PermisoTransporte),
                    permiso.FechaEmision,
                    permiso.Estado,
                    null,
                    null,
                    null,
                    null,
                    permiso.CondicionInstitucional,
                    permiso.FechaVencimiento),

                _ => new(
                    autorizacion.Id,
                    autorizacion.UsuarioTransporteId,
                    autorizacion.GetType().Name,
                    autorizacion.FechaEmision,
                    autorizacion.Estado,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
            };
        }

        private static AutorizacionDto MapearAutorizacion(AutorizacionTransporte autorizacion)
        {
            return autorizacion switch
            {
                TicketMensual ticket => new(
                    ticket.Id,
                    ticket.UsuarioTransporteId,
                    nameof(TicketMensual),
                    ticket.FechaEmision,
                    ticket.Estado,
                    ticket.FechaInicio,
                    ticket.FechaFin,
                    null,
                    null,
                    null,
                    null),

                TarjetaRecargable tarjeta => new(
                    tarjeta.Id,
                    tarjeta.UsuarioTransporteId,
                    nameof(TarjetaRecargable),
                    tarjeta.FechaEmision,
                    tarjeta.Estado,
                    null,
                    null,
                    tarjeta.NumeroTarjeta,
                    tarjeta.SaldoDisponible,
                    null,
                    null),

                PermisoTransporte permiso => new(
                    permiso.Id,
                    permiso.UsuarioTransporteId,
                    nameof(PermisoTransporte),
                    permiso.FechaEmision,
                    permiso.Estado,
                    null,
                    null,
                    null,
                    null,
                    permiso.CondicionInstitucional,
                    permiso.FechaVencimiento),

                _ => new(
                    autorizacion.Id,
                    autorizacion.UsuarioTransporteId,
                    autorizacion.GetType().Name,
                    autorizacion.FechaEmision,
                    autorizacion.Estado,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
            };
        }
    }
}
