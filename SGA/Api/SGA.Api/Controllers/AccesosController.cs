using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.DTOs.Accesos;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/accesos")]

    public sealed class AccesosController : ControllerBase
    {
        private readonly IAccesoService _accesoService;

        public AccesosController(IAccesoService accesoService)
            => _accesoService = accesoService;

        
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegistrarAccesoDto dto)
            => this.AResultadoCreado(await _accesoService.RegistrarAccesoAsync(dto));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
            => this.AResultado(await _accesoService.ObtenerPorIdAsync(id));

        [HttpGet("por-usuario/{usuarioId:int}")]
        public async Task<IActionResult> ListarPorUsuario(int usuarioId)
            => this.AResultado(await _accesoService.ListarPorUsuarioAsync(usuarioId));

        [HttpGet("por-viaje/{viajeId:int}")]
        public async Task<IActionResult> ListarPorViaje(int viajeId)
            => this.AResultado(await _accesoService.ListarPorViajeAsync(viajeId));

        [HttpGet("por-periodo")]
        public async Task<IActionResult> ListarPorPeriodo(
            [FromQuery] DateTime desde, [FromQuery] DateTime hasta)
            => this.AResultado(await _accesoService.ListarPorPeriodoAsync(desde, hasta));
    }
}
