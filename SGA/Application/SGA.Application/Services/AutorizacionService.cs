using SGA.Application.Common;
using SGA.Application.DTOs.Autorizaciones;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Error;
using SGA.Domain.Models.Autorizaciones;
using SGA.Domain.Models.Pagos;
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

        public async Task<Result<AutorizacionResumenDto>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            var validacion = ValidationGeneral.IdValido(usuarioId, "usuario");
            if (validacion.EsFallo)
                return Result<AutorizacionResumenDto>.Fallo(validacion.Error!);

            var autorizacion = await ObtenerAutorizacionActivaAsync(usuarioId);

            return autorizacion is null
                ? Result<AutorizacionResumenDto>.Fallo(ApplicationErrors.NoEncontrado("la autorizacion del usuario"))
                : Result<AutorizacionResumenDto>.Ok(MapearResumen(autorizacion));
        }


        public async Task<Result<AutorizacionResumenDto>> ObtenerPorIdAsync(int autorizacionId)
        {
            var autorizacion = await _autorizacionRepository.GetByIdAsync(autorizacionId);
            return autorizacion is null
                ? Result<AutorizacionResumenDto>.Fallo(ApplicationErrors.NoEncontrado("la autorizacion"))
                : Result<AutorizacionResumenDto>.Ok(MapearResumen(autorizacion));
        }

        public async Task<Result<IReadOnlyList<AutorizacionResumenDto>>> ListarVigentesAsync()
        {
            var autorizaciones = await _autorizacionRepository.GetVigentes();
            return Result<IReadOnlyList<AutorizacionResumenDto>>.Ok(autorizaciones.Select(MapearResumen).ToList());
        }

        public async Task<Result<IReadOnlyList<AutorizacionResumenDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta)
        {
            var validacion = ValidationGeneral.RangoFechasValido(desde, hasta, "autorizaciones");
            if (validacion.EsFallo)
                return Result<IReadOnlyList<AutorizacionResumenDto>>.Fallo(validacion.Error!);

            var autorizaciones = await _autorizacionRepository.GetbyPeriodo(desde, hasta);
            return Result<IReadOnlyList<AutorizacionResumenDto>>.Ok(autorizaciones.Select(MapearResumen).ToList());
        }

        public async Task<Result<TicketDiarioDto>> EmitirTicketDiarioAsync(CrearTicketDiarioDto dto)
        {
            var usuarioValido = await ValidarUsuarioActivoAsync(dto.UsuarioTransporteId);
            if (usuarioValido.EsFallo)
                return Result<TicketDiarioDto>.Fallo(usuarioValido.Error!);

            var autorizacionActual = await ObtenerAutorizacionActivaAsync(dto.UsuarioTransporteId);
            if (autorizacionActual is not null)
                return Result<TicketDiarioDto>.Fallo(ApplicationErrors.OperacionInvalida("El usuario ya tiene una autorizacion activa."));

            var pago = ConvertirPago(await _pagoRepository.GetPagoSinAutorizacion(dto.UsuarioTransporteId));
            var ticketCreado = AutorizacionRules.CrearTicketDiario(pago, dto.UsuarioTransporteId, dto.FechaInicio);

            if (ticketCreado.EsFallo)
                return Result<TicketDiarioDto>.Fallo(ticketCreado.Error!);

            var ticket = ticketCreado.Valor!;
            ticket.CreadoPor = dto.CreadoPor;

            await _autorizacionRepository.AddAsync(ticket);
            await MarcarPagoComoAplicadoAsync(pago!);

            return Result<TicketDiarioDto>.Ok(MapearTicketDiario(ticket));
        }

        public async Task<Result<TarjetaRecargableDto>> EmitirTarjetaRecargableAsync(CrearTarjetaRecargableDto dto)
        {
            var usuarioValido = await ValidarUsuarioActivoAsync(dto.UsuarioTransporteId);
            if (usuarioValido.EsFallo)
                return Result<TarjetaRecargableDto>.Fallo(usuarioValido.Error!);

            var autorizacionActual = await ObtenerAutorizacionActivaAsync(dto.UsuarioTransporteId);
            if (autorizacionActual is not null)
                return Result<TarjetaRecargableDto>.Fallo(ApplicationErrors.OperacionInvalida("El usuario ya tiene una autorizacion activa."));

            var pago = ConvertirPago(await _pagoRepository.GetPagoSinAutorizacion(dto.UsuarioTransporteId));
            var tarjetaCreada = AutorizacionRules.CrearTarjetaRecargable(
                pago, dto.UsuarioTransporteId, dto.SaldoInicial, dto.NumeroTarjeta);

            if (tarjetaCreada.EsFallo)
                return Result<TarjetaRecargableDto>.Fallo(tarjetaCreada.Error!);

            var tarjeta = tarjetaCreada.Valor!;
            tarjeta.CreadoPor = dto.CreadoPor;

            await _autorizacionRepository.AddAsync(tarjeta);
            await MarcarPagoComoAplicadoAsync(pago!);

            return Result<TarjetaRecargableDto>.Ok(MapearTarjeta(tarjeta));
        }

        public async Task<Result<PermisoTransporteDto>> EmitirPermisoAsync(CrearPermisoTransporteDto dto)
        {
            var usuarioValido = await ValidarUsuarioActivoAsync(dto.UsuarioTransporteId);
            if (usuarioValido.EsFallo)
                return Result<PermisoTransporteDto>.Fallo(usuarioValido.Error!);

            var autorizacionActual = await ObtenerAutorizacionActivaAsync(dto.UsuarioTransporteId);
            if (autorizacionActual is not null)
                return Result<PermisoTransporteDto>.Fallo(ApplicationErrors.OperacionInvalida("El usuario ya tiene una autorizacion activa."));

            var permisoCreado = AutorizacionRules.CrearPermiso(
                dto.UsuarioTransporteId, dto.CondicionInstitucional, dto.FechaVencimiento);

            if (permisoCreado.EsFallo)
                return Result<PermisoTransporteDto>.Fallo(permisoCreado.Error!);

            var permiso = permisoCreado.Valor!;
            permiso.CreadoPor = dto.CreadoPor;

            await _autorizacionRepository.AddAsync(permiso);
            return Result<PermisoTransporteDto>.Ok(MapearPermiso(permiso));
        }

        public async Task<Result> AnularAsync(int autorizacionId, AnularAutorizacionDto dto)
        {
            var validacion = ValidationGeneral.IdValido(autorizacionId, "autorizacion");
            if (validacion.EsFallo)
                return Result.Fallo(validacion.Error!);

            var autorizacionModel = await _autorizacionRepository.GetByIdAsync(autorizacionId);
            if (autorizacionModel is null)
                return Result.Fallo(ApplicationErrors.NoEncontrado("la autorizacion"));

            var autorizacion = ConvertirAutorizacion(autorizacionModel);
            var anulacion = AutorizacionRules.Anular(autorizacion);
            if (anulacion.EsFallo)
                return anulacion;

            autorizacion.EliminadoPor = dto.AnuladoPor;
            await _autorizacionRepository.UpdateAsync(autorizacion);
            return Result.Ok();
        }

        private async Task<Result> ValidarUsuarioActivoAsync(int usuarioId)
        {
            var validacion = ValidationGeneral.IdValido(usuarioId, "usuario");
            if (validacion.EsFallo)
                return validacion;

            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario is null)
                return Result.Fallo(ApplicationErrors.NoEncontrado("el usuario"));

            return UsuarioBaseRules.EstaActivo(usuario.Estado)
                ? Result.Ok()
                : Result.Fallo(DomainErrors.Accesos.UsuarioInactivo);
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
                return null;

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
                TicketDiarioModel ticket => new TicketDiario { FechaInicio = ticket.FechaInicio, FechaFin = ticket.FechaFin },
                TarjetaRecargableModel tarjeta => new TarjetaRecargable { NumeroTarjeta = tarjeta.NumeroTarjeta, SaldoDisponible = tarjeta.SaldoDisponible },
                PermisoTransporteModel permiso => new PermisoTransporte { CondicionInstitucional = permiso.CondicionInstitucional, FechaVencimiento = permiso.FechaVencimiento },
                _ => throw new InvalidOperationException($"Tipo de autorizacion desconocido: {model.GetType().Name}")
            };

            autorizacion.Id = model.Id;
            autorizacion.UsuarioTransporteId = model.UsuarioTransporteId;
            autorizacion.FechaEmision = model.FechaEmision;
            autorizacion.Estado = model.Estado;
            return autorizacion;
        }

        private static AutorizacionResumenDto MapearResumen(AutorizacionModel a) => new(
            a.Id, a.UsuarioTransporteId,
            a switch
            {
                TicketDiarioModel => "TicketDiario",
                TarjetaRecargableModel => "TarjetaRecargable",
                PermisoTransporteModel => "PermisoTransporte",
                _ => a.GetType().Name
            },
            a.FechaEmision, a.Estado);

        private static TicketDiarioDto MapearTicketDiario(TicketDiario t) =>
            new(t.Id, t.UsuarioTransporteId, t.FechaEmision, t.Estado, t.FechaInicio, t.FechaFin);

        private static TarjetaRecargableDto MapearTarjeta(TarjetaRecargable t) =>
            new(t.Id, t.UsuarioTransporteId, t.FechaEmision, t.Estado, t.NumeroTarjeta, t.SaldoDisponible);

        private static PermisoTransporteDto MapearPermiso(PermisoTransporte p) =>
            new(p.Id, p.UsuarioTransporteId, p.FechaEmision, p.Estado, p.CondicionInstitucional, p.FechaVencimiento);
    }
}
