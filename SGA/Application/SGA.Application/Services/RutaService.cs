using SGA.Application.Common;
using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Horarios;
using SGA.Application.DTOs.Paradas;
using SGA.Application.DTOs.Rutas;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Error;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Rules;

namespace SGA.Application.Services
{
    public sealed class RutaService : IRutaService
    {
        private readonly IRutaRepository _rutaRepository;

        public RutaService(IRutaRepository rutaRepository) => _rutaRepository = rutaRepository;

        public async Task<Result<IReadOnlyList<RutaDto>>> ListarTodasAsync()
        {
            var rutas = await _rutaRepository.GetAllAsync();
            return Result<IReadOnlyList<RutaDto>>.Ok(rutas.Select(MapearRuta).ToList());
        }

        public async Task<Result<IReadOnlyList<RutaDto>>> ListarActivasAsync()
        {
            var rutas = await _rutaRepository.GetActivas();
            return Result<IReadOnlyList<RutaDto>>.Ok(rutas.Select(MapearRuta).ToList());
        }

        public async Task<Result<RutaDetalleDto>> ObtenerDetalleAsync(int rutaId)
        {
            var ruta = await _rutaRepository.GetByIdAsync(rutaId);
            if (ruta is null)
                return Result<RutaDetalleDto>.Fallo(ApplicationErrors.NoEncontrado("la ruta"));

            var paradas = await _rutaRepository.GetParadas(rutaId);
            var horarios = await _rutaRepository.GetHorarios(rutaId);

            return Result<RutaDetalleDto>.Ok(new RutaDetalleDto(
                MapearRuta(ruta),
                paradas.Select(MapearParada).ToList(),
                horarios.Select(MapearHorario).ToList()));
        }


        public async Task<Result<RutaDto>> ObtenerPorIdAsync(int rutaId)
        {
            var ruta = await _rutaRepository.GetByIdAsync(rutaId);
            return ruta is null
                ? Result<RutaDto>.Fallo(ApplicationErrors.NoEncontrado("la ruta"))
                : Result<RutaDto>.Ok(MapearRuta(ruta));
        }

        public async Task<Result<RutaDto>> CrearAsync(CrearRutaDto dto)
        {
            var ruta = new Ruta
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activa = dto.Activa,
                CreadoPor = dto.CreadoPor
            };

            var validacion = RutaRules.Validar(ruta); // la regla de negocio vive en Domain, el Service solo la consulta
            if (validacion.EsFallo)
                return Result<RutaDto>.Fallo(validacion.Error!);

            await _rutaRepository.AddAsync(ruta);
            return Result<RutaDto>.Ok(MapearRuta(ruta));
        }

        public async Task<Result<RutaDto>> ActualizarAsync(int rutaId, ActualizarRutaDto dto)
        {
            var rutaActual = await _rutaRepository.GetByIdAsync(rutaId);
            if (rutaActual is null)
                return Result<RutaDto>.Fallo(ApplicationErrors.NoEncontrado("la ruta"));

            var ruta = new Ruta
            {
                Id = rutaId,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activa = dto.Activa
            };

            var validacion = RutaRules.Validar(ruta);
            if (validacion.EsFallo)
                return Result<RutaDto>.Fallo(validacion.Error!);

            await _rutaRepository.UpdateAsync(ruta);
            return Result<RutaDto>.Ok(MapearRuta(ruta));
        }

        public async Task<Result> EliminarAsync(int rutaId, EliminarDto dto)
        {
            var rutaActual = await _rutaRepository.GetByIdAsync(rutaId);
            if (rutaActual is null)
                return Result.Fallo(ApplicationErrors.NoEncontrado("la ruta"));

            var ruta = new Ruta
            {
                Id = rutaId,
                Eliminado = true,
                FechaEliminacion = DateTime.UtcNow,
                EliminadoPor = dto.EliminadoPor
            };

            await _rutaRepository.DeleteAsync(ruta);
            return Result.Ok();
        }

        private static RutaDto MapearRuta(Ruta ruta) => new(
            ruta.Id, 
            ruta.Nombre, 
            ruta.Descripcion, 
            ruta.Activa
        );
        private static RutaDto MapearRuta(RutaModel ruta) => new(
            ruta.Id,
            ruta.Nombre,
            ruta.Descripcion,
            ruta.Activa
        );
        private static ParadaDto MapearParada(ParadaModel p) => new(
            p.Id, 
            p.RutaId,
            p.Nombre,
            p.Referencia,
            p.Orden
        );
        private static HorarioRutaDto MapearHorario(HorarioModel h) => new(
            h.Id, 
            h.RutaId,
            h.HoraSalida,
            h.HoraLlegadaEstimada,
            h.Activo
        );
    }
}