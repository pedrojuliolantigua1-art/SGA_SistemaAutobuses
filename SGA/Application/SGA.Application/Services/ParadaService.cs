using SGA.Application.Common;
using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Paradas;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Error;
using SGA.Domain.Models.Transporte;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Rules;

namespace SGA.Application.Services
{
    public sealed class ParadaService : IParadaService
    {
        private readonly IParadaRepository _paradaRepository;

        public ParadaService(IParadaRepository paradaRepository) => _paradaRepository = paradaRepository;

        public async Task<Result<IReadOnlyList<ParadaDto>>> ListarPorRutaAsync(int rutaId)
        {
            var paradas = await _paradaRepository.GetByRuta(rutaId);
            return Result<IReadOnlyList<ParadaDto>>.Ok(paradas.Select(MapearParada).ToList());
        }


        public async Task<Result<ParadaDto>> ObtenerPorIdAsync(int paradaId)
        {
            var parada = await _paradaRepository.GetByIdAsync(paradaId);
            return parada is null
                ? Result<ParadaDto>.Fallo(ApplicationErrors.NoEncontrado("la parada"))
                : Result<ParadaDto>.Ok(MapearParada(parada));
        }

        public async Task<Result<ParadaDto>> CrearAsync(CrearParadaDto dto)
        {
            var parada = new Parada
            {
                RutaId = dto.RutaId,
                Nombre = dto.Nombre,
                Referencia = dto.Referencia,
                Orden = dto.Orden,
                CreadoPor = dto.CreadoPor
            };

            var validacion = ParadaRules.Validar(parada);
            if (validacion.EsFallo)
                return Result<ParadaDto>.Fallo(validacion.Error!);

            await _paradaRepository.AddAsync(parada);
            return Result<ParadaDto>.Ok(MapearParada(parada));
        }

        public async Task<Result<ParadaDto>> ActualizarAsync(int paradaId, ActualizarParadaDto dto)
        {
            var actual = await _paradaRepository.GetByIdAsync(paradaId);
            if (actual is null)
                return Result<ParadaDto>.Fallo(ApplicationErrors.NoEncontrado("la parada"));

            var parada = new Parada { Id = paradaId, RutaId = actual.RutaId, Nombre = dto.Nombre, Referencia = dto.Referencia, Orden = dto.Orden };

            var validacion = ParadaRules.Validar(parada);
            if (validacion.EsFallo)
                return Result<ParadaDto>.Fallo(validacion.Error!);

            await _paradaRepository.UpdateAsync(parada);
            return Result<ParadaDto>.Ok(MapearParada(parada));
        }

        public async Task<Result> ReordenarAsync(ReordenarParadasDto dto)
        {
            var paradasActuales = await _paradaRepository.GetByRuta(dto.RutaId);
            if (paradasActuales.Count != dto.Orden.Count)
                return Result.Fallo(ApplicationErrors.OperacionInvalida("El listado de orden no coincide con las paradas de la ruta."));

            var candidatas = paradasActuales.Select(p =>
            {
                var nuevoOrden = dto.Orden.First(o => o.ParadaId == p.Id).NuevoOrden;
                return new Parada { Id = p.Id, RutaId = p.RutaId, Nombre = p.Nombre, Referencia = p.Referencia, Orden = nuevoOrden };
            }).ToList();

            var validacion = ParadaRules.ValidarOrdenUnico(candidatas);
            if (validacion.EsFallo)
                return validacion;

            foreach (var parada in candidatas)
                await _paradaRepository.UpdateAsync(parada);

            return Result.Ok();
        }

        public async Task<Result> EliminarAsync(int paradaId, EliminarDto dto)
        {
            var actual = await _paradaRepository.GetByIdAsync(paradaId);
            if (actual is null)
                return Result.Fallo(ApplicationErrors.NoEncontrado("la parada"));

            var parada = new Parada { Id = paradaId, Eliminado = true, FechaEliminacion = DateTime.UtcNow, EliminadoPor = dto.EliminadoPor };
            await _paradaRepository.DeleteAsync(parada);
            return Result.Ok();
        }

        private static ParadaDto MapearParada(ParadaModel p) => new(
            p.Id,
            p.RutaId,
            p.Nombre,
            p.Referencia,
            p.Orden
        );

        private static ParadaDto MapearParada(Parada p) => new(
            p.Id,
            p.RutaId,
            p.Nombre,
            p.Referencia,
            p.Orden
        );
    }
}