using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.DTOs.Viajes;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/viajes")]
    public sealed class ViajesController : ControllerBase
    {
        private readonly IViajeService _viajeService;

        public ViajesController(IViajeService viajeService)
            => _viajeService = viajeService;

        /// <summary>Obtiene un viaje por id.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
            => this.AResultado(await _viajeService.ObtenerPorIdAsync(id));

        /// <summary>Lista los viajes actualmente en curso.</summary>
        [HttpGet("activos")]
        public async Task<IActionResult> ListarActivos()
            => this.AResultado(await _viajeService.ListarActivosAsync());

        /// <summary>Lista los viajes programados aun no iniciados.</summary>
        [HttpGet("programados")]
        public async Task<IActionResult> ListarProgramados()
            => this.AResultado(await _viajeService.ListarProgramadosAsync());

        /// <summary>Lista los viajes de una ruta.</summary>
        [HttpGet("por-ruta/{rutaId:int}")]
        public async Task<IActionResult> ListarPorRuta(int rutaId)
            => this.AResultado(await _viajeService.ListarPorRutaAsync(rutaId));

        /// <summary>Lista los viajes de una fecha determinada.</summary>
        [HttpGet("por-fecha")]
        public async Task<IActionResult> ListarPorFecha([FromQuery] DateTime fecha)
            => this.AResultado(await _viajeService.ListarPorFechaAsync(fecha));

        /// <summary>Lista los viajes asignados a un conductor.</summary>
        [HttpGet("por-conductor/{conductorId:int}")]
        public async Task<IActionResult> ListarPorConductor(int conductorId)
            => this.AResultado(await _viajeService.ListarPorConductorAsync(conductorId));

        /// <summary>Programa un viaje nuevo.</summary>
        [HttpPost]
        public async Task<IActionResult> Programar([FromBody] ProgramarViajeDto dto)
            => this.AResultadoCreado(await _viajeService.ProgramarAsync(dto));

        /// <summary>Marca el inicio real de un viaje.</summary>
        [HttpPost("iniciar")]
        public async Task<IActionResult> Iniciar([FromBody] EjecutarViajeDto dto)
            => this.AResultado(await _viajeService.IniciarAsync(dto));

        /// <summary>Marca la finalizacion de un viaje.</summary>
        [HttpPost("finalizar")]
        public async Task<IActionResult> Finalizar([FromBody] EjecutarViajeDto dto)
            => this.AResultado(await _viajeService.FinalizarAsync(dto));

        /// <summary>Cancela un viaje programado.</summary>
        [HttpPost("cancelar")]
        public async Task<IActionResult> Cancelar([FromBody] CancelarViajeDto dto)
            => this.AResultado(await _viajeService.CancelarAsync(dto));

        /// <summary>Reporta una incidencia ocurrida durante un viaje.</summary>
        [HttpPost("incidencias")]
        public async Task<IActionResult> ReportarIncidencia([FromBody] ReportarIncidenciaDto dto)
            => this.AResultadoCreado(await _viajeService.ReportarIncidenciaAsync(dto));
    }
}
