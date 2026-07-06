using SGA.Application.Common;
using SGA.Application.DTOs.Auditoria;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Error;
using SGA.Domain.Models.Auditoria;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Validation;

namespace SGA.Application.Services
{
    public sealed class AuditoriaService : IAuditoriaService
    {
        private readonly IAuditoriaRepository _auditoriaRepository;

        public AuditoriaService(IAuditoriaRepository auditoriaRepository)
            => _auditoriaRepository = auditoriaRepository;

        public async Task<Result<IReadOnlyList<AuditoriaDto>>> ListarPorPeriodoAsync(DateTime desde, DateTime hasta)
        {
            var validacion = ValidationGeneral.RangoFechasValido(desde, hasta, "auditoria");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<AuditoriaDto>>.Fallo(validacion.Error!);
            }

            var registros = await _auditoriaRepository.GetbyPeriodo(desde, hasta);
            return Result<IReadOnlyList<AuditoriaDto>>.Ok(registros.Select(Mapear).ToList());
        }


        public async Task<Result<AuditoriaDto>> ObtenerPorIdAsync(int registroId)
        {
            var registro = await _auditoriaRepository.GetByIdAsync(registroId);
            return registro is null
                ? Result<AuditoriaDto>.Fallo(ApplicationErrors.NoEncontrado("el registro de auditoria"))
                : Result<AuditoriaDto>.Ok(Mapear(registro));
        }

        public async Task<Result<IReadOnlyList<AuditoriaDto>>> ListarPorActorAsync(int usuarioId)
        {
            var validacion = ValidationGeneral.IdValido(usuarioId, "usuario");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<AuditoriaDto>>.Fallo(validacion.Error!);
            }

            var registros = await _auditoriaRepository.GetByActor(usuarioId);
            return Result<IReadOnlyList<AuditoriaDto>>.Ok(registros.Select(Mapear).ToList());
        }

        public async Task<Result<IReadOnlyList<AuditoriaDto>>> ListarPorAccionAsync(string accion)
        {
            var validacion = ValidationGeneral.Requerido(accion, "accion");

            if (validacion.EsFallo)
            {
                return Result<IReadOnlyList<AuditoriaDto>>.Fallo(validacion.Error!);
            }

            var registros = await _auditoriaRepository.GetbyAccion(accion);
            return Result<IReadOnlyList<AuditoriaDto>>.Ok(registros.Select(Mapear).ToList());
        }

        private static AuditoriaDto Mapear(AuditoriaModel m) =>
            new(m.Id, m.UsuarioTransporteId, m.Accion, m.EntidadAfectada, m.EntidadId, m.Detalle, m.FechaHora);
    }
}
