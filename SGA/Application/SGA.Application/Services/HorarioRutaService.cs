
using SGA.Application.Common;
using SGA.Application.DTOs.Autobuses;
using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Horarios;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Error;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Rules;

namespace SGA.Application.Services
{
    public sealed class HorarioRutaService : IHorarioRutaService
    {
        private readonly IHorarioRutaRepository _horarioRepository;

        public HorarioRutaService(IHorarioRutaRepository horarioRepository) 
            => _horarioRepository = horarioRepository;

        public async Task<Result<IReadOnlyList<HorarioRutaDto>>> ListarPorRutaAsync(int rutaId)
        {
            var horarios = await _horarioRepository.GetByRuta(rutaId);
            return Result<IReadOnlyList<HorarioRutaDto>>.Ok(horarios.Select(MapearHorario).ToList());
        }


        public async Task<Result<HorarioRutaDto>> ObtenerPorIdAsync(int horarioId)
        {
            var horario = await _horarioRepository.GetByIdAsync(horarioId);
            return horario is null
                ? Result<HorarioRutaDto>.Fallo(ApplicationErrors.NoEncontrado("el horario"))
                : Result<HorarioRutaDto>.Ok(MapearHorario(horario));
        }

        public async Task<Result<HorarioRutaDto>> CrearAsync(CrearHorarioRutaDto dto)
        {
            var horario = new HorarioRuta
            {
                RutaId = dto.RutaId,
                HoraSalida = dto.HoraSalida,
                HoraLlegadaEstimada = dto.HoraLlegadaEstimada,
                Activo = true,
                CreadoPor = dto.CreadoPor
            };

            var validacion = HorarioRules.Validar(horario);
            if (validacion.EsFallo)
                return Result<HorarioRutaDto>.Fallo(validacion.Error!);

            await _horarioRepository.AddAsync(horario);
            return Result<HorarioRutaDto>.Ok(MapearHorario(horario));
        }

        public async Task<Result<HorarioRutaDto>> ActualizarAsync(int horarioId, ActualizarHorarioRutaDto dto)
        {
            var actual = await _horarioRepository.GetByIdAsync(horarioId);
            if (actual is null)
                return Result<HorarioRutaDto>.Fallo(ApplicationErrors.NoEncontrado("el horario"));

            var horario = new HorarioRuta
            {
                Id = horarioId,
                RutaId = actual.RutaId,
                HoraSalida = dto.HoraSalida,
                HoraLlegadaEstimada = dto.HoraLlegadaEstimada,
                Activo = dto.Activo
            };

            var validacion = HorarioRules.Validar(horario);
            if (validacion.EsFallo)
                return Result<HorarioRutaDto>.Fallo(validacion.Error!);

            await _horarioRepository.UpdateAsync(horario);
            return Result<HorarioRutaDto>.Ok(MapearHorario(horario));
        }

        public async Task<Result> EliminarAsync(int horarioId, EliminarDto dto)
        {
            var actual = await _horarioRepository.GetByIdAsync(horarioId);
            if (actual is null)
                return Result.Fallo(ApplicationErrors.NoEncontrado("el horario"));

            var horario = new HorarioRuta { Id = horarioId, Eliminado = true, FechaEliminacion = DateTime.UtcNow, EliminadoPor = dto.EliminadoPor };
            await _horarioRepository.DeleteAsync(horario);
            return Result.Ok();
        }

        public async Task<Result<IReadOnlyList<HorarioRutaDto>>> ListarEliminadosAsync()
        {
            var horario = await _horarioRepository.GetEliminados();
            return Result<IReadOnlyList<HorarioRutaDto>>.Ok(horario.Select(MapearHorario).ToList());
        }

        private static HorarioRutaDto MapearHorario(HorarioModel h) => new(h.Id, h.RutaId, h.HoraSalida, h.HoraLlegadaEstimada, h.Activo);
        private static HorarioRutaDto MapearHorario(HorarioRuta h) => new(h.Id, h.RutaId, h.HoraSalida, h.HoraLlegadaEstimada, h.Activo);

        
    }
}