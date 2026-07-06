using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.DTOs.Pagos;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/pagos")]
    public sealed class PagosController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagosController(IPagoService pagoService)
            => _pagoService = pagoService;

        /// <summary>Registra un pago realizado por un usuario.</summary>
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegistrarPagoDto dto)
            => this.AResultadoCreado(await _pagoService.RegistrarAsync(dto));

        /// <summary>Obtiene un pago por id.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
            => this.AResultado(await _pagoService.ObtenerPorIdAsync(id));

        /// <summary>Lista los pagos registrados por un usuario.</summary>
        [HttpGet("por-usuario/{usuarioId:int}")]
        public async Task<IActionResult> ListarPorUsuario(int usuarioId)
            => this.AResultado(await _pagoService.ListarPorUsuarioAsync(usuarioId));

        /// <summary>Lista los pagos registrados en un periodo (conciliacion).</summary>
        [HttpGet("por-periodo")]
        public async Task<IActionResult> ListarPorPeriodo(
            [FromQuery] DateTime desde, [FromQuery] DateTime hasta)
            => this.AResultado(await _pagoService.ListarPorPeriodoAsync(desde, hasta));
    }
}
