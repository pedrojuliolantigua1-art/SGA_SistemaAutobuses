using SGA.Application.Common;
using SGA.Application.DTOs.Accesos;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Accesos;
using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Enum;
using SGA.Domain.Error;
using SGA.Domain.Models.Accesos;
using SGA.Domain.Models.Autorizaciones;
using SGA.Domain.Models.Usuarios;
using SGA.Domain.Models.Viajes;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Rules;
using SGA.Domain.Rules.Accesos;
using SGA.Domain.Validation;

namespace SGA.Application.Services
{
    public sealed class AccesoService : IAccesoService
    {
        private readonly IAccesoRepository _accesoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAutorizacionRepository _autorizacionRepository;
        private readonly IViajeRepository _viajeRepository;

        public AccesoService(
            IAccesoRepository accesoRepository,
            IUsuarioRepository usuarioRepository,
            IAutorizacionRepository autorizacionRepository,
            IViajeRepository viajeRepository)
        {
            _accesoRepository = accesoRepository;
            _usuarioRepository = usuarioRepository;
            _autorizacionRepository = autorizacionRepository;
            _viajeRepository = viajeRepository;
        }

        public async Task<Result<AccesoDto>> RegistrarAccesoAsync(RegistrarAccesoDto dto)
        {
            var datosValidos = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(dto.UsuarioTransporteId, "usuario"),
                ValidationGeneral.IdValido(dto.ViajeId, "viaje"),
                ValidationGeneral.IdValido(dto.ValidadoPorUsuarioId, "usuario validador"),
                ValidationGeneral.FechaDefinida(dto.FechaHora, "acceso"),
                ValidationGeneral.MontoPositivo(dto.CostoViaje, "costo del viaje"));

            if (datosValidos.EsFallo)
            {
                return Result<AccesoDto>.Fallo(datosValidos.Error!);
            }

            var usuarioModel = await _usuarioRepository.GetByIdAsync(dto.UsuarioTransporteId);
            var viajeModel = await _viajeRepository.GetByIdAsync(dto.ViajeId);

            if (usuarioModel is null)
            {
                return Result<AccesoDto>.Fallo(ApplicationErrors.NoEncontrado("el usuario"));
            }

            if (viajeModel is null)
            {
                return Result<AccesoDto>.Fallo(ApplicationErrors.NoEncontrado("el viaje"));
            }

            var usuario = ConvertirUsuario(usuarioModel);
            var viaje = ConvertirViaje(viajeModel);
            var autorizacion = ConvertirAutorizacion(await ObtenerAutorizacionActivaAsync(dto.UsuarioTransporteId));

            var registroCreado = AccesoRules.CrearRegistroDesdeEvaluacion(
                usuario,
                autorizacion,
                viaje,
                dto.ValidadoPorUsuarioId,
                dto.FechaHora,
                dto.CostoViaje);

            if (registroCreado.EsFallo)
            {
                return Result<AccesoDto>.Fallo(registroCreado.Error!);
            }

            var registro = registroCreado.Valor!;
            registro.CreadoPor = dto.CreadoPor;
            registro.FechaCreacion = DateTime.UtcNow;

            if (registro.ResultadoAcceso == ResultadoAcceso.Permitido)
            {
                var consumo = AutorizacionRules.ConsumirAutorizacion(autorizacion, dto.FechaHora, dto.CostoViaje);

                if (consumo.EsFallo)
                {
                    return Result<AccesoDto>.Fallo(consumo.Error!);
                }

                viaje.CupoActual++;
                viaje.FechaModificacion = DateTime.UtcNow;

                if (autorizacion is TarjetaRecargable)
                {
                    await _autorizacionRepository.UpdateAsync(autorizacion);
                }

                await _viajeRepository.UpdateAsync(viaje);
            }

            await _accesoRepository.AddAsync(registro);
            return Result<AccesoDto>.Ok(MapearAcceso(registro));
        }

        public async Task<Result<IReadOnlyList<AccesoDto>>> ListarPorUsuarioAsync(int usuarioId)
        {
            var validacion = ValidationGeneral.IdValido(usuarioId, "usuario");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<AccesoDto>>.Fallo(validacion.Error!);
            }

            var accesos = await _accesoRepository.GetByUsuario(usuarioId);
            return Result<IReadOnlyList<AccesoDto>>.Ok(accesos.Select(MapearAcceso).ToList());
        }

        public async Task<Result<IReadOnlyList<AccesoDto>>> ListarPorViajeAsync(int viajeId)
        {
            var validacion = ValidationGeneral.IdValido(viajeId, "viaje");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<AccesoDto>>.Fallo(validacion.Error!);
            }

            var accesos = await _accesoRepository.GetByViaje(viajeId);
            return Result<IReadOnlyList<AccesoDto>>.Ok(accesos.Select(MapearAcceso).ToList());
        }

        public async Task<Result<IReadOnlyList<AccesoDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta)
        {
            var validacion = ValidationGeneral.RangoFechasValido(desde, hasta, "accesos");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<AccesoDto>>.Fallo(validacion.Error!);
            }

            var accesos = await _accesoRepository.GetByPeriodo(desde, hasta);
            return Result<IReadOnlyList<AccesoDto>>.Ok(accesos.Select(MapearAcceso).ToList());
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

        private static UsuarioTransporte ConvertirUsuario(UsuarioModel model)
        {
            UsuarioTransporte usuario = model switch
            {
                EstudianteModel estudiante => new Estudiante
                {
                    Matricula = estudiante.Matricula,
                    Carrera = estudiante.Carrera,
                    TipoUsuario = "Estudiante"
                },
                EmpleadoModel empleado => new Empleado
                {
                    CodigoEmpleado = empleado.CodigoEmpleado,
                    Departamento = empleado.Departamento,
                    Cargo = empleado.Cargo,
                    TipoUsuario = "Empleado"
                },
                ConductorModel conductor => new Conductor
                {
                    NumeroLicencia = conductor.NumeroLicencia,
                    Disponible = conductor.Disponible,
                    TipoUsuario = "Conductor"
                },
                _ => new Estudiante { TipoUsuario = model.GetType().Name }
            };

            usuario.Id = model.Id;
            usuario.Nombre = model.Nombre;
            usuario.Apellido = model.Apellido;
            usuario.Correo = model.Correo;
            usuario.Telefono = model.Telefono;
            usuario.Estado = model.Estado;
            usuario.RolSistema = model.RolSistema;
            return usuario;
        }

        private static AutorizacionTransporte? ConvertirAutorizacion(AutorizacionModel? model)
        {
            if (model is null)
            {
                return null;
            }

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

        private static Viaje ConvertirViaje(ViajeModel model) => new()
        {
            Id = model.Id,
            RutaId = model.RutaId,
            HorarioRutaId = model.HorarioRutaId,
            AutobusId = model.AutobusId,
            ConductorId = model.ConductorId,
            Fecha = model.Fecha,
            Estado = model.Estado,
            HoraInicioReal = model.HoraInicioReal,
            HoraFinReal = model.HoraFinReal,
            CupoActual = model.CupoActual,
            CapacidadMaxima = model.CapacidadMaxima
        };

        private static AccesoDto MapearAcceso(AccesoModel acceso) =>
            new(
                acceso.Id,
                acceso.UsuarioTransporteId,
                acceso.ViajeId,
                acceso.AutorizacionTransporteId,
                acceso.ResultadoAcceso,
                acceso.MotivoRechazo,
                acceso.FechaHora,
                acceso.ValidadoPorUsuarioId);

        private static AccesoDto MapearAcceso(RegistroUsoTransporte acceso) =>
            new(
                acceso.Id,
                acceso.UsuarioTransporteId,
                acceso.ViajeId,
                acceso.AutorizacionTransporteId,
                acceso.ResultadoAcceso,
                acceso.MotivoRechazo,
                acceso.FechaHora,
                acceso.ValidadoPorUsuarioId);
    }
}
