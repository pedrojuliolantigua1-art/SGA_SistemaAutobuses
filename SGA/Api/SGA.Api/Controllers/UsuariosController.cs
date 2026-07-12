using Microsoft.AspNetCore.Mvc;
using SGA.Api.Common;
using SGA.Application.DTOs.Common;
using SGA.Application.DTOs.Usuarios;
using SGA.Application.Interfaces.Services;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public sealed class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
            => _usuarioService = usuarioService;

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
            => this.AResultado(await _usuarioService.ListarTodosAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
            => this.AResultado(await _usuarioService.ObtenerPorIdAsync(id));

        [HttpGet("por-correo")]
        public async Task<IActionResult> ObtenerPorCorreo([FromQuery] string correo)
            => this.AResultado(await _usuarioService.ObtenerPorCorreoAsync(correo));

        [HttpGet("estudiantes/por-matricula")]
        public async Task<IActionResult> ObtenerEstudiantePorMatricula([FromQuery] string matricula)
            => this.AResultado(await _usuarioService.ObtenerEstudiantePorMatriculaAsync(matricula));

        [HttpGet("conductores/por-licencia")]
        public async Task<IActionResult> ObtenerConductorPorLicencia([FromQuery] string numeroLicencia)
            => this.AResultado(await _usuarioService.ObtenerConductorPorLicenciaAsync(numeroLicencia));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, [FromBody] EliminarDto dto)
            => this.AResultado(await _usuarioService.EliminarAsync(id, dto));


        [HttpPost("estudiantes")]
        public async Task<IActionResult> RegistrarEstudiante([FromBody] CrearEstudianteDto dto)
            => this.AResultadoCreado(await _usuarioService.RegistrarEstudianteAsync(dto));

        [HttpPut("estudiantes/{id:int}")]
        public async Task<IActionResult> ActualizarEstudiante(int id, [FromBody] ActualizarEstudianteDto dto)
            => this.AResultado(await _usuarioService.ActualizarEstudianteAsync(id, dto));

        [HttpPost("empleados/docentes")]
        public async Task<IActionResult> RegistrarEmpleadoDocente([FromBody] CrearEmpleadoDocenteDto dto)
            => this.AResultadoCreado(await _usuarioService.RegistrarEmpleadoDocenteAsync(dto));

        [HttpPut("empleados/docentes/{id:int}")]
        public async Task<IActionResult> ActualizarEmpleadoDocente(int id, [FromBody] ActualizarEmpleadoDocenteDto dto)
            => this.AResultado(await _usuarioService.ActualizarEmpleadoDocenteAsync(id, dto));

        [HttpPost("empleados/administrativos")]
        public async Task<IActionResult> RegistrarEmpleadoAdministrativo([FromBody] CrearEmpleadoAdministrativoDto dto)
            => this.AResultadoCreado(await _usuarioService.RegistrarEmpleadoAdministrativoAsync(dto));

        [HttpPut("empleados/administrativos/{id:int}")]
        public async Task<IActionResult> ActualizarEmpleadoAdministrativo(int id, [FromBody] ActualizarEmpleadoAdministrativoDto dto)
            => this.AResultado(await _usuarioService.ActualizarEmpleadoAdministrativoAsync(id, dto));

        [HttpPost("conductores")]
        public async Task<IActionResult> RegistrarConductor([FromBody] CrearConductorDto dto)
            => this.AResultadoCreado(await _usuarioService.RegistrarConductorAsync(dto));

        [HttpPut("conductores/{id:int}")]
        public async Task<IActionResult> ActualizarConductor(int id, [FromBody] ActualizarConductorDto dto)
            => this.AResultado(await _usuarioService.ActualizarConductorAsync(id, dto));

        [HttpPatch("conductores/{id:int}/disponibilidad")]
        public async Task<IActionResult> CambiarDisponibilidad(int id, [FromBody] CambiarDisponibilidadConductorDto dto)
            => this.AResultado(await _usuarioService.CambiarDisponibilidadAsync(id, dto));
    }
}
