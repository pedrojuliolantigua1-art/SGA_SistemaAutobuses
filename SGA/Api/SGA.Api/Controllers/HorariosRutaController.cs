using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Horarios;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class HorariosRutaController : ControllerBase
    {
        private readonly IHorarioRutaService _horarioRutaService;

        public HorariosRutaController(IHorarioRutaService horarioRutaService)
            => _horarioRutaService = horarioRutaService;

        [HttpGet]
        public async Task<IActionResult> ListarPorRuta([FromQuery] int rutaId)
            => this.AResultado(await _horarioRutaService.ListarPorRutaAsync(rutaId));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
            => this.AResultado(await _horarioRutaService.ObtenerPorIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearHorarioRutaDto dto)
            => this.AResultadoCreado(await _horarioRutaService.CrearAsync(dto));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarHorarioRutaDto dto)
            => this.AResultado(await _horarioRutaService.ActualizarAsync(id, dto));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, [FromBody] EliminarDto dto)
            => this.AResultado(await _horarioRutaService.EliminarAsync(id, dto));

        [HttpPost("{id:int}/restaurar")]
        public async Task<IActionResult> Restaurar(int id, [FromBody] RestaurarDto dto)
            => this.AResultado(await _horarioRutaService.RestaurarAsync(id, dto));
    }
}
