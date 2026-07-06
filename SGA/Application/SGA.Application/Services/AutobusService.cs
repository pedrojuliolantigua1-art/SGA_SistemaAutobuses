
using SGA.Application.Common;
using SGA.Application.DTOs.Autobuses;
using SGA.Application.DTOs.Common;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Error;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Rules;

namespace SGA.Application.Services
{
    public sealed class AutobusService : IAutobusService
    {
        private readonly IAutobusRepository _autobusRepository;

        public AutobusService(IAutobusRepository autobusRepository) => _autobusRepository = autobusRepository;

        public async Task<Result<IReadOnlyList<AutobusDto>>> ListarTodosAsync()
        {
            var autobuses = await _autobusRepository.GetAllAsync();
            return Result<IReadOnlyList<AutobusDto>>.Ok(autobuses.Select(MapearAutobus).ToList());
        }

        public async Task<Result<IReadOnlyList<AutobusDto>>> ListarDisponiblesAsync()
        {
            var autobuses = await _autobusRepository.GetDisponibles();
            return Result<IReadOnlyList<AutobusDto>>.Ok(autobuses.Select(MapearAutobus).ToList());
        }


        public async Task<Result<AutobusDto>> ObtenerPorIdAsync(int autobusId)
        {
            var autobus = await _autobusRepository.GetByIdAsync(autobusId);
            return autobus is null
                ? Result<AutobusDto>.Fallo(ApplicationErrors.NoEncontrado("el autobus"))
                : Result<AutobusDto>.Ok(MapearAutobus(autobus));
        }

        public async Task<Result<AutobusDto>> ObtenerPorPlacaAsync(string placa)
        {
            var autobus = await _autobusRepository.GetByPlaca(placa);
            return autobus is null
                ? Result<AutobusDto>.Fallo(ApplicationErrors.NoEncontrado("el autobus"))
                : Result<AutobusDto>.Ok(MapearAutobus(autobus));
        }

        public async Task<Result<AutobusDto>> CrearAsync(CrearAutobusDto dto)
        {
            var autobus = new Autobus
            {
                Placa = dto.Placa,
                Modelo = dto.Modelo,
                Capacidad = dto.Capacidad,
                Estado = "Disponible",
                CreadoPor = dto.CreadoPor
            };

            var validacion = AutobusRules.Validar(autobus);
            if (validacion.EsFallo)
                return Result<AutobusDto>.Fallo(validacion.Error!);

            await _autobusRepository.AddAsync(autobus);
            return Result<AutobusDto>.Ok(MapearAutobus(autobus));
        }

        public async Task<Result<AutobusDto>> ActualizarAsync(int autobusId, ActualizarAutobusDto dto)
        {
            var actual = await _autobusRepository.GetByIdAsync(autobusId);
            if (actual is null)
                return Result<AutobusDto>.Fallo(ApplicationErrors.NoEncontrado("el autobus"));

            var autobus = new Autobus { Id = autobusId, Placa = dto.Placa, Modelo = dto.Modelo, Capacidad = dto.Capacidad, Estado = actual.Estado };

            var validacion = AutobusRules.Validar(autobus);
            if (validacion.EsFallo)
                return Result<AutobusDto>.Fallo(validacion.Error!);

            await _autobusRepository.UpdateAsync(autobus);
            return Result<AutobusDto>.Ok(MapearAutobus(autobus));
        }

        public async Task<Result<AutobusDto>> CambiarEstadoAsync(int autobusId, CambiarEstadoAutobusDto dto)
        {
            var actual = await _autobusRepository.GetByIdAsync(autobusId);
            if (actual is null)
                return Result<AutobusDto>.Fallo(ApplicationErrors.NoEncontrado("el autobus"));

            var autobus = new Autobus { Id = autobusId, Placa = actual.Placa, Modelo = actual.Modelo, Capacidad = actual.Capacidad, Estado = dto.NuevoEstado };
            await _autobusRepository.UpdateAsync(autobus);
            return Result<AutobusDto>.Ok(MapearAutobus(autobus));
        }

        public async Task<Result> EliminarAsync(int autobusId, EliminarDto dto)
        {
            var actual = await _autobusRepository.GetByIdAsync(autobusId);
            if (actual is null)
                return Result.Fallo(ApplicationErrors.NoEncontrado("el autobus"));

            var autobus = new Autobus { Id = autobusId, Eliminado = true, FechaEliminacion = DateTime.UtcNow, EliminadoPor = dto.EliminadoPor };
            await _autobusRepository.DeleteAsync(autobus);
            return Result.Ok();
        }

        public async Task<Result> RestaurarAsync(int autobusId, RestaurarDto dto)
        {
            var actual = await _autobusRepository.GetByIdAsync(autobusId);
            if (actual is null)
                return Result.Fallo(ApplicationErrors.NoEncontrado("el autobus"));

            var autobus = new Autobus { Id = autobusId, Eliminado = false, FechaEliminacion = null, EliminadoPor = null };
            await _autobusRepository.UpdateAsync(autobus);
            return Result.Ok();
        }

        private static AutobusDto MapearAutobus(AutobusModel a) => new(a.Id, a.Placa, a.Modelo, a.Capacidad, a.Estado);
        private static AutobusDto MapearAutobus(Autobus a) => new(a.Id, a.Placa, a.Modelo, a.Capacidad, a.Estado);
    }
}