using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.DTOs.Autorizaciones;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/autorizaciones")]
    public sealed class AutorizacionesController : ControllerBase
    {
        private readonly IAutorizacionService _autorizacionService;

        public AutorizacionesController(IAutorizacionService autorizacionService)
            => _autorizacionService = autorizacionService;

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
            => this.AResultado(await _autorizacionService.ObtenerPorIdAsync(id));

        [HttpGet("tipos")]
        public IActionResult ListarTipos()
            => Ok(new[] { "TicketDiario", "TarjetaRecargable", "PermisoTransporte" });

        [HttpGet("por-usuario/{usuarioId:int}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
            => this.AResultado(await _autorizacionService.ObtenerPorUsuarioAsync(usuarioId));

        [HttpGet("vigentes")]
        public async Task<IActionResult> ListarVigentes()
            => this.AResultado(await _autorizacionService.ListarVigentesAsync());

        [HttpGet("por-periodo")]
        public async Task<IActionResult> ListarPorPeriodo(
            [FromQuery] DateTime desde, [FromQuery] DateTime hasta)
            => this.AResultado(await _autorizacionService.ListarPorPeriodoAsync(desde, hasta));

        [HttpPost("ticket-diario")]
        public async Task<IActionResult> EmitirTicketDiario([FromBody] CrearTicketDiarioDto dto)
            => this.AResultadoCreado(await _autorizacionService.EmitirTicketDiarioAsync(dto));

        [HttpPost("tarjeta-recargable")]
        public async Task<IActionResult> EmitirTarjetaRecargable([FromBody] CrearTarjetaRecargableDto dto)
            => this.AResultadoCreado(await _autorizacionService.EmitirTarjetaRecargableAsync(dto));

        [HttpPost("permiso")]
        public async Task<IActionResult> EmitirPermiso([FromBody] CrearPermisoTransporteDto dto)
            => this.AResultadoCreado(await _autorizacionService.EmitirPermisoAsync(dto));

        [HttpPost("{id:int}/anular")]
        public async Task<IActionResult> Anular(int id, [FromBody] AnularAutorizacionDto dto)
            => this.AResultado(await _autorizacionService.AnularAsync(id, dto));
    }
}
