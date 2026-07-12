using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.DTOs.Autobuses;
using SGA.Application.DTOs.Common;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AutobusesController : ControllerBase
    {
        private readonly IAutobusService _autobusService;
        private readonly IViajeService _viajeService;

        public AutobusesController(IAutobusService autobusService, IViajeService viajeService)
        {
            _autobusService = autobusService;
            _viajeService = viajeService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
            => this.AResultado(await _autobusService.ListarTodosAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
            => this.AResultado(await _autobusService.ObtenerPorIdAsync(id));

        [HttpGet("por-placa")]
        public async Task<IActionResult> ObtenerPorPlaca([FromQuery] string placa)
            => this.AResultado(await _autobusService.ObtenerPorPlacaAsync(placa));

        [HttpGet("{id:int}/viajes")]
        public async Task<IActionResult> ListarViajes(int id)
            => this.AResultado(await _viajeService.ListarPorAutobusAsync(id));

        [HttpGet("disponibles")]
        public async Task<IActionResult> ListarDisponibles()
            => this.AResultado(await _autobusService.ListarDisponiblesAsync());

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearAutobusDto dto)
            => this.AResultadoCreado(await _autobusService.CrearAsync(dto));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarAutobusDto dto)

            => this.AResultado(await _autobusService.ActualizarAsync(id, dto));

        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoAutobusDto dto)
            => this.AResultado(await _autobusService.CambiarEstadoAsync(id, dto));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, [FromBody] EliminarDto dto)
            => this.AResultado(await _autobusService.EliminarAsync(id, dto));

        

    }
}
