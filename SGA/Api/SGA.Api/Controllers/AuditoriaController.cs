using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/auditoria")]
    public sealed class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriaController(IAuditoriaService auditoriaService)
            => _auditoriaService = auditoriaService;

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
            => this.AResultado(await _auditoriaService.ObtenerPorIdAsync(id));

        [HttpGet("por-periodo")]
        public async Task<IActionResult> ListarPorPeriodo(
            [FromQuery] DateTime desde, [FromQuery] DateTime hasta)
            => this.AResultado(await _auditoriaService.ListarPorPeriodoAsync(desde, hasta));

        [HttpGet("por-actor/{usuarioId:int}")]
        public async Task<IActionResult> ListarPorActor(int usuarioId)
            => this.AResultado(await _auditoriaService.ListarPorActorAsync(usuarioId));

        [HttpGet("por-accion")]
        public async Task<IActionResult> ListarPorAccion([FromQuery] string accion)
            => this.AResultado(await _auditoriaService.ListarPorAccionAsync(accion));
    }
}
