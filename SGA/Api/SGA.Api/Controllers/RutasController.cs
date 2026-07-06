using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Rutas;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/rutas")]
    public sealed class RutasController : ControllerBase
    {
        private readonly IRutaService _rutaService;

        public RutasController(IRutaService rutaService)
            => _rutaService = rutaService;

        /// <summary>Lista todas las rutas registradas.</summary>
        [HttpGet]
        public async Task<IActionResult> ListarTodas()
            => this.AResultado(await _rutaService.ListarTodasAsync());

        /// <summary>Lista solo las rutas activas.</summary>
        [HttpGet("activas")]
        public async Task<IActionResult> ListarActivas()
            => this.AResultado(await _rutaService.ListarActivasAsync());

        /// <summary>Obtiene una ruta por id.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
            => this.AResultado(await _rutaService.ObtenerPorIdAsync(id));

        /// <summary>Obtiene una ruta con sus paradas y horarios.</summary>
        [HttpGet("{id:int}/detalle")]
        public async Task<IActionResult> ObtenerDetalle(int id)
            => this.AResultado(await _rutaService.ObtenerDetalleAsync(id));

        /// <summary>Crea una ruta nueva.</summary>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearRutaDto dto)
            => this.AResultadoCreado(await _rutaService.CrearAsync(dto));

        /// <summary>Actualiza una ruta existente.</summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarRutaDto dto)
            => this.AResultado(await _rutaService.ActualizarAsync(id, dto));

        /// <summary>Elimina (logicamente) una ruta.</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, [FromBody] EliminarDto dto)
            => this.AResultado(await _rutaService.EliminarAsync(id, dto));

        [HttpPost("{id:int}/restaurar")]
        public async Task<IActionResult> Restaurar(int id, [FromBody] RestaurarDto dto)
            => this.AResultado(await _rutaService.RestaurarAsync(id, dto));
    }
}
