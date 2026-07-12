using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Paradas;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/paradas")]
    public sealed class ParadasController : ControllerBase
    {
        private readonly IParadaService _paradaService;

        public ParadasController(IParadaService paradaService)
            => _paradaService = paradaService;

        [HttpGet]
        public async Task<IActionResult> ListarPorRuta([FromQuery] int rutaId)
            => this.AResultado(await _paradaService.ListarPorRutaAsync(rutaId));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
            => this.AResultado(await _paradaService.ObtenerPorIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearParadaDto dto)
            => this.AResultadoCreado(await _paradaService.CrearAsync(dto));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarParadaDto dto)
            => this.AResultado(await _paradaService.ActualizarAsync(id, dto));

        [HttpPut("reordenar")]
        public async Task<IActionResult> Reordenar([FromBody] ReordenarParadasDto dto)
            => this.AResultado(await _paradaService.ReordenarAsync(dto));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, [FromBody] EliminarDto dto)
            => this.AResultado(await _paradaService.EliminarAsync(id, dto));

    }
}
