using SGA.Application.Common;
using SGA.Application.DTOs.Rutas;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Error;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Validation;

namespace SGA.Application.Services
{
    public sealed class RutaService : IRutaService
    {
        private readonly IRutaRepository _rutaRepository;

        public RutaService(IRutaRepository rutaRepository)
        {
            _rutaRepository = rutaRepository;
        }

        // trae todas las rutas registradas en el sistema
        public async Task<Result<IReadOnlyList<RutaDto>>> ListarTodasAsync()
        {
            var rutas = await _rutaRepository.GetAllAsync();
            return Result<IReadOnlyList<RutaDto>>.Ok(rutas.Select(MapearRuta).ToList());
        }

        // trae solo las rutas que estan activas para mostrarlas al usuario
        public async Task<Result<IReadOnlyList<RutaDto>>> ListarActivasAsync()
        {
            var rutas = await _rutaRepository.GetActivas();
            return Result<IReadOnlyList<RutaDto>>.Ok(rutas.Select(MapearRuta).ToList());
        }

        // busca una ruta junto con sus paradas y horarios
        public async Task<Result<RutaDetalleDto>> ObtenerDetalleAsync(int rutaId)
        {
            var validacion = ValidationGeneral.IdValido(rutaId, "ruta");

            if (validacion.EsFallo)
            {
                return Result<RutaDetalleDto>.Fallo(validacion.Error!);
            }

            var ruta = await _rutaRepository.GetByIdAsync(rutaId);

            if (ruta is null)
            {
                return Result<RutaDetalleDto>.Fallo(ApplicationErrors.NoEncontrado("la ruta"));
            }

            var paradas = await _rutaRepository.GetParadas(rutaId);
            var horarios = await _rutaRepository.GetHorarios(rutaId);

            var detalle = new RutaDetalleDto(
                MapearRuta(ruta),
                paradas.Select(MapearParada).ToList(),
                horarios.Select(MapearHorario).ToList());

            return Result<RutaDetalleDto>.Ok(detalle);
        }

        // crea una ruta basica para el catalogo de transporte
        public async Task<Result<RutaDto>> CrearAsync(CrearRutaDto dto)
        {
            var validacion = ValidationGeneral.RequeridoConLongitud(dto.Nombre, "nombre de la ruta", min: 3, max: 100);

            if (validacion.EsFallo)
            {
                return Result<RutaDto>.Fallo(validacion.Error!);
            }

            var ruta = new Ruta
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activa = dto.Activa,
                FechaCreacion = DateTime.UtcNow,
                CreadoPor = dto.CreadoPor
            };

            await _rutaRepository.AddAsync(ruta);
            return Result<RutaDto>.Ok(MapearRuta(ruta));
        }

        // actualiza los datos principales de una ruta existente
        public async Task<Result<RutaDto>> ActualizarAsync(ActualizarRutaDto dto)
        {
            var validacion = ValidationGeneral.Combinar(
                ValidationGeneral.IdValido(dto.Id, "ruta"),
                ValidationGeneral.RequeridoConLongitud(dto.Nombre, "nombre de la ruta", min: 3, max: 100));

            if (validacion.EsFallo)
            {
                return Result<RutaDto>.Fallo(validacion.Error!);
            }

            var rutaActual = await _rutaRepository.GetByIdAsync(dto.Id);

            if (rutaActual is null)
            {
                return Result<RutaDto>.Fallo(ApplicationErrors.NoEncontrado("la ruta"));
            }

            var ruta = new Ruta
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activa = dto.Activa,
                FechaModificacion = DateTime.UtcNow
            };

            await _rutaRepository.UpdateAsync(ruta);
            return Result<RutaDto>.Ok(MapearRuta(ruta));
        }

        // elimina logicamente una ruta usando el repositorio existente
        public async Task<Result> EliminarAsync(int rutaId, string? eliminadoPor)
        {
            var validacion = ValidationGeneral.IdValido(rutaId, "ruta");

            if (validacion.EsFallo)
            {
                return Result.Fallo(validacion.Error!);
            }

            var rutaActual = await _rutaRepository.GetByIdAsync(rutaId);

            if (rutaActual is null)
            {
                return Result.Fallo(ApplicationErrors.NoEncontrado("la ruta"));
            }

            var ruta = new Ruta
            {
                Id = rutaId,
                Eliminado = true,
                FechaEliminacion = DateTime.UtcNow,
                EliminadoPor = eliminadoPor
            };

            await _rutaRepository.DeleteAsync(ruta);
            return Result.Ok();
        }

        private static RutaDto MapearRuta(Ruta ruta) =>
            new(ruta.Id, ruta.Nombre, ruta.Descripcion, ruta.Activa);

        private static RutaDto MapearRuta(RutaModel ruta) =>
            new(ruta.Id, ruta.Nombre, ruta.Descripcion, ruta.Activa);

        private static ParadaDto MapearParada(ParadaModel parada) =>
            new(parada.Id, parada.RutaId, parada.Nombre, parada.Referencia, parada.Orden);

        private static HorarioRutaDto MapearHorario(HorarioModel horario) =>
            new(horario.Id, horario.RutaId, horario.HoraSalida, horario.HoraLlegadaEstimada, horario.Activo);
    }
}
