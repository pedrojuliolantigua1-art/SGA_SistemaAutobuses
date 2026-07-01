using SGA.Application.Common;
using SGA.Application.DTOs.Viajes;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Entities.Viajes;
using SGA.Domain.Error;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Models.Usuarios;
using SGA.Domain.Models.Viajes;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Rules;
using SGA.Domain.Validation;

namespace SGA.Application.Services
{
    public sealed class ViajeService : IViajeService
    {
        private readonly IViajeRepository _viajeRepository;
        private readonly IRutaRepository _rutaRepository;
        private readonly IHorarioRutaRepository _horarioRutaRepository;
        private readonly IAutobusRepository _autobusRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ViajeService(
            IViajeRepository viajeRepository,
            IRutaRepository rutaRepository,
            IHorarioRutaRepository horarioRutaRepository,
            IAutobusRepository autobusRepository,
            IUsuarioRepository usuarioRepository)
        {
            _viajeRepository = viajeRepository;
            _rutaRepository = rutaRepository;
            _horarioRutaRepository = horarioRutaRepository;
            _autobusRepository = autobusRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Result<IReadOnlyList<ViajeDto>>> ListarPorFechaAsync(DateTime fecha)
        {
            var validacion = ValidationGeneral.FechaDefinida(fecha, "viaje");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<ViajeDto>>.Fallo(validacion.Error!);
            }

            var viajes = await _viajeRepository.GetbyFecha(fecha.Date);
            return Result<IReadOnlyList<ViajeDto>>.Ok(viajes.Select(MapearViaje).ToList());
        }

        public async Task<Result<IReadOnlyList<ViajeDto>>> ListarPorConductorAsync(int conductorId)
        {
            var validacion = ValidationGeneral.IdValido(conductorId, "conductor");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<ViajeDto>>.Fallo(validacion.Error!);
            }

            var viajes = await _viajeRepository.GetbyConductor(conductorId);
            return Result<IReadOnlyList<ViajeDto>>.Ok(viajes.Select(MapearViaje).ToList());
        }

        public async Task<Result<ViajeDto>> ProgramarAsync(ProgramarViajeDto dto)
        {
            var datosValidos = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(dto.RutaId, "ruta"),
                ValidationGeneral.IdValido(dto.HorarioRutaId, "horario"),
                ValidationGeneral.IdValido(dto.AutobusId, "autobus"),
                ValidationGeneral.IdValido(dto.ConductorId, "conductor"),
                ValidationGeneral.FechaDefinida(dto.Fecha, "viaje"));

            if (datosValidos.EsFallo)
            {
                return Result<ViajeDto>.Fallo(datosValidos.Error!);
            }

            var rutaModel = await _rutaRepository.GetByIdAsync(dto.RutaId);
            var horarioModel = await _horarioRutaRepository.GetByIdAsync(dto.HorarioRutaId);
            var autobusModel = await _autobusRepository.GetByIdAsync(dto.AutobusId);
            var conductorModel = await _usuarioRepository.GetByIdAsync(dto.ConductorId);

            if (rutaModel is null)
            {
                return Result<ViajeDto>.Fallo(ApplicationErrors.NoEncontrado("la ruta"));
            }

            if (horarioModel is null)
            {
                return Result<ViajeDto>.Fallo(ApplicationErrors.NoEncontrado("el horario"));
            }

            if (autobusModel is null)
            {
                return Result<ViajeDto>.Fallo(ApplicationErrors.NoEncontrado("el autobus"));
            }

            if (conductorModel is null)
            {
                return Result<ViajeDto>.Fallo(ApplicationErrors.NoEncontrado("el conductor"));
            }

            var viajesDelDia = (await _viajeRepository.GetbyFecha(dto.Fecha.Date))
                .Select(ConvertirViaje)
                .ToList();

            var viajeCreado = ViajePlanificacionRules.Crear(
                ConvertirRuta(rutaModel),
                ConvertirHorario(horarioModel),
                ConvertirAutobus(autobusModel),
                ConvertirConductor(conductorModel),
                dto.Fecha,
                viajesDelDia);

            if (viajeCreado.EsFallo)
            {
                return Result<ViajeDto>.Fallo(viajeCreado.Error!);
            }

            var viaje = viajeCreado.Valor!;
            viaje.FechaCreacion = DateTime.UtcNow;
            viaje.CreadoPor = dto.CreadoPor;

            await _viajeRepository.AddAsync(viaje);
            return Result<ViajeDto>.Ok(MapearViaje(viaje));
        }

        public async Task<Result<ViajeDto>> IniciarAsync(EjecutarViajeDto dto)
        {
            var datosValidos = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(dto.ViajeId, "viaje"),
                ValidationGeneral.IdValido(dto.ConductorId, "conductor"));

            if (datosValidos.EsFallo)
            {
                return Result<ViajeDto>.Fallo(datosValidos.Error!);
            }

            var viajeModel = await _viajeRepository.GetByIdAsync(dto.ViajeId);

            if (viajeModel is null)
            {
                return Result<ViajeDto>.Fallo(ApplicationErrors.NoEncontrado("el viaje"));
            }

            var viaje = ConvertirViaje(viajeModel);
            var validacion = ViajeEjecucionRules.Iniciar(viaje, dto.ConductorId, dto.FechaHora);

            if (validacion.EsFallo)
            {
                return Result<ViajeDto>.Fallo(validacion.Error!);
            }

            viaje.FechaModificacion = DateTime.UtcNow;

            await _viajeRepository.UpdateAsync(viaje);
            return Result<ViajeDto>.Ok(MapearViaje(viaje));
        }

        public async Task<Result<ViajeDto>> FinalizarAsync(EjecutarViajeDto dto)
        {
            var datosValidos = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(dto.ViajeId, "viaje"),
                ValidationGeneral.IdValido(dto.ConductorId, "conductor"));

            if (datosValidos.EsFallo)
            {
                return Result<ViajeDto>.Fallo(datosValidos.Error!);
            }

            var viajeModel = await _viajeRepository.GetByIdAsync(dto.ViajeId);

            if (viajeModel is null)
            {
                return Result<ViajeDto>.Fallo(ApplicationErrors.NoEncontrado("el viaje"));
            }

            var viaje = ConvertirViaje(viajeModel);
            var validacion = ViajeEjecucionRules.Finalizar(viaje, dto.ConductorId, dto.FechaHora);

            if (validacion.EsFallo)
            {
                return Result<ViajeDto>.Fallo(validacion.Error!);
            }

            viaje.FechaModificacion = DateTime.UtcNow;

            await _viajeRepository.UpdateAsync(viaje);
            return Result<ViajeDto>.Ok(MapearViaje(viaje));
        }

        public async Task<Result<ViajeDto>> CancelarAsync(CancelarViajeDto dto)
        {
            var idValido = ValidationGeneral.IdValido(dto.ViajeId, "viaje");

            if (idValido.EsFallo)
            {
                return Result<ViajeDto>.Fallo(idValido.Error!);
            }

            var viajeModel = await _viajeRepository.GetByIdAsync(dto.ViajeId);

            if (viajeModel is null)
            {
                return Result<ViajeDto>.Fallo(ApplicationErrors.NoEncontrado("el viaje"));
            }

            var viaje = ConvertirViaje(viajeModel);
            var validacion = ViajeEjecucionRules.Cancelar(viaje, dto.Motivo);

            if (validacion.EsFallo)
            {
                return Result<ViajeDto>.Fallo(validacion.Error!);
            }

            viaje.FechaModificacion = DateTime.UtcNow;

            await _viajeRepository.UpdateAsync(viaje);
            return Result<ViajeDto>.Ok(MapearViaje(viaje));
        }

        public async Task<Result<IncidenciaDto>> ReportarIncidenciaAsync(ReportarIncidenciaDto dto)
        {
            var datosValidos = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(dto.ViajeId, "viaje"),
                ValidationGeneral.IdValido(dto.ConductorId, "conductor"));

            if (datosValidos.EsFallo)
            {
                return Result<IncidenciaDto>.Fallo(datosValidos.Error!);
            }

            var viajeModel = await _viajeRepository.GetByIdAsync(dto.ViajeId);

            if (viajeModel is null)
            {
                return Result<IncidenciaDto>.Fallo(ApplicationErrors.NoEncontrado("el viaje"));
            }

            var viaje = ConvertirViaje(viajeModel);
            var estadoAnterior = viaje.Estado;
            var incidenciaCreada = ViajeEjecucionRules.ReportarIncidencia(
                viaje,
                dto.ConductorId,
                dto.Tipo,
                dto.Descripcion,
                dto.FechaHora);

            if (incidenciaCreada.EsFallo)
            {
                return Result<IncidenciaDto>.Fallo(incidenciaCreada.Error!);
            }

            var incidencia = incidenciaCreada.Valor!;
            incidencia.CreadoPor = dto.CreadoPor;
            incidencia.FechaCreacion = DateTime.UtcNow;

            if (viaje.Estado != estadoAnterior)
            {
                viaje.FechaModificacion = DateTime.UtcNow;
                await _viajeRepository.UpdateAsync(viaje);
            }

            await _viajeRepository.AddIncidencia(incidencia);
            return Result<IncidenciaDto>.Ok(MapearIncidencia(incidencia));
        }

        private static Ruta ConvertirRuta(RutaModel model) => new()
        {
            Id = model.Id,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            Activa = model.Activa
        };

        private static HorarioRuta ConvertirHorario(HorarioModel model) => new()
        {
            Id = model.Id,
            RutaId = model.RutaId,
            HoraSalida = model.HoraSalida,
            HoraLlegadaEstimada = model.HoraLlegadaEstimada,
            Activo = model.Activo
        };

        private static Autobus ConvertirAutobus(AutobusModel model) => new()
        {
            Id = model.Id,
            Placa = model.Placa,
            Modelo = model.Modelo,
            Capacidad = model.Capacidad,
            Estado = model.Estado
        };

        private static Conductor? ConvertirConductor(UsuarioModel model)
        {
            if (model is not ConductorModel conductor)
            {
                return null;
            }

            return new Conductor
            {
                Id = conductor.Id,
                Nombre = conductor.Nombre,
                Apellido = conductor.Apellido,
                Correo = conductor.Correo,
                Telefono = conductor.Telefono,
                Estado = conductor.Estado,
                RolSistema = conductor.RolSistema,
                TipoUsuario = "Conductor",
                NumeroLicencia = conductor.NumeroLicencia,
                Disponible = conductor.Disponible
            };
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

        private static ViajeDto MapearViaje(ViajeModel viaje) =>
            new(
                viaje.Id,
                viaje.RutaId,
                viaje.HorarioRutaId,
                viaje.AutobusId,
                viaje.ConductorId,
                viaje.Fecha,
                viaje.Estado,
                viaje.HoraInicioReal,
                viaje.HoraFinReal,
                viaje.CupoActual,
                viaje.CapacidadMaxima);

        private static ViajeDto MapearViaje(Viaje viaje) =>
            new(
                viaje.Id,
                viaje.RutaId,
                viaje.HorarioRutaId,
                viaje.AutobusId,
                viaje.ConductorId,
                viaje.Fecha,
                viaje.Estado,
                viaje.HoraInicioReal,
                viaje.HoraFinReal,
                viaje.CupoActual,
                viaje.CapacidadMaxima);

        private static IncidenciaDto MapearIncidencia(Incidencia incidencia) =>
            new(
                incidencia.Id,
                incidencia.ViajeId,
                incidencia.ConductorId,
                incidencia.Tipo,
                incidencia.Descripcion,
                incidencia.FechaHora);
    }
}
