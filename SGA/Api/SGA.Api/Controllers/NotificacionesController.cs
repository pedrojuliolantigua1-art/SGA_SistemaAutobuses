using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.DTOs.Notificaciones;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/notificaciones")]
    public sealed class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionesController(INotificacionService notificacionService)
            => _notificacionService = notificacionService;

        /// <summary>Crea una notificacion (uso administrativo).</summary>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearNotificacionDto dto)
            => this.AResultadoCreado(await _notificacionService.CrearAsync(dto));

        /// <summary>Lista las notificaciones de un usuario.</summary>
        [HttpGet("por-usuario/{usuarioId:int}")]
        public async Task<IActionResult> ListarPorUsuario(int usuarioId)
            => this.AResultado(await _notificacionService.ListarPorUsuarioAsync(usuarioId));

        /// <summary>Lista las notificaciones generadas en un periodo.</summary>
        [HttpGet("por-periodo")]
        public async Task<IActionResult> ListarPorPeriodo(
            [FromQuery] DateTime desde, [FromQuery] DateTime hasta)
            => this.AResultado(await _notificacionService.ListarPorPeriodoAsync(desde, hasta));

        /// <summary>Marca una notificacion como leida.</summary>
        [HttpPost("{id:int}/marcar-leida")]
        public async Task<IActionResult> MarcarComoLeida(int id)
            => this.AResultado(await _notificacionService.MarcarComoLeidaAsync(id));
    }
}
