using SGA.Application.Common;
using SGA.Application.DTOs.Usuarios;
using SGA.Application.Interfaces.Services;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Error;
using SGA.Domain.Models.Usuarios;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Rules;
using SGA.Domain.Validation;

namespace SGA.Application.Services
{
    public sealed class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
            => _usuarioRepository = usuarioRepository;

        public async Task<Result<IReadOnlyList<UsuarioDto>>> ListarTodosAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return Result<IReadOnlyList<UsuarioDto>>.Ok(usuarios.Select(Mapear).ToList());
        }

        public async Task<Result<UsuarioDto>> ObtenerPorIdAsync(int id)
        {
            var validacion = ValidationGeneral.IdValido(id, "usuario");

            if (validacion.EsFallo)
            {
                return Result<UsuarioDto>.Fallo(validacion.Error!);
            }

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            return usuario is null
                ? Result<UsuarioDto>.Fallo(ApplicationErrors.NoEncontrado("el usuario"))
                : Result<UsuarioDto>.Ok(Mapear(usuario));
        }

        public async Task<Result<UsuarioDto>> ObtenerPorCorreoAsync(string correo)
        {
            var validacion = UsuarioBaseRules.ValidarCorreoInstitucional(correo);

            if (validacion.EsFallo)
            {
                return Result<UsuarioDto>.Fallo(validacion.Error!);
            }

            var usuario = await _usuarioRepository.GetbyCorreo(correo);
            return usuario is null
                ? Result<UsuarioDto>.Fallo(ApplicationErrors.NoEncontrado("el usuario"))
                : Result<UsuarioDto>.Ok(Mapear(usuario));
        }

        public async Task<Result<UsuarioDto>> RegistrarAsync(CrearUsuarioDto dto)
        {
            var entidadCreada = ConstruirEntidad(dto);

            if (entidadCreada.EsFallo)
            {
                return Result<UsuarioDto>.Fallo(entidadCreada.Error!);
            }

            var entity = entidadCreada.Valor!;
            var validacion = ValidationGeneral.Combinar(
                ValidarUsuario(entity),
                ValidationGeneral.Requerido(dto.PasswordHash, "password"));

            if (validacion.EsFallo)
            {
                return Result<UsuarioDto>.Fallo(validacion.Error!);
            }

            var existente = await _usuarioRepository.GetbyCorreo(dto.Correo!);
            if (existente is not null)
                return Result<UsuarioDto>.Fallo(ApplicationErrors.OperacionInvalida("Ya existe un usuario con ese correo."));

            await _usuarioRepository.AddAsync(entity);
            return Result<UsuarioDto>.Ok(MapearEntidad(entity));
        }

        public async Task<Result<UsuarioDto>> ActualizarAsync(ActualizarUsuarioDto dto)
        {
            var idValido = ValidationGeneral.IdValido(dto.Id, "usuario");

            if (idValido.EsFallo)
            {
                return Result<UsuarioDto>.Fallo(idValido.Error!);
            }

            var existe = await _usuarioRepository.GetByIdAsync(dto.Id);
            if (existe is null) return Result<UsuarioDto>.Fallo(ApplicationErrors.NoEncontrado("el usuario"));

            var entity = ConvertirAEntidad(existe);
            entity.Nombre = dto.Nombre;
            entity.Apellido = dto.Apellido;
            entity.Correo = dto.Correo;
            entity.Telefono = dto.Telefono;
            entity.Estado = dto.Estado;
            entity.FechaModificacion = DateTime.UtcNow;

            var validacion = ValidationGeneral.Combinar(
                ValidarUsuario(entity),
                ValidationGeneral.Requerido(dto.Estado, "estado"));

            if (validacion.EsFallo)
            {
                return Result<UsuarioDto>.Fallo(validacion.Error!);
            }

            await _usuarioRepository.UpdateAsync(entity);
            return Result<UsuarioDto>.Ok(MapearEntidad(entity));
        }

        public async Task<Result> DesactivarAsync(int id, string? eliminadoPor)
        {
            var validacion = ValidationGeneral.IdValido(id, "usuario");

            if (validacion.EsFallo)
            {
                return Result.Fallo(validacion.Error!);
            }

            var existe = await _usuarioRepository.GetByIdAsync(id);
            if (existe is null) return Result.Fallo(ApplicationErrors.NoEncontrado("el usuario"));

            var entity = new Estudiante { Id = id, EliminadoPor = eliminadoPor };
            await _usuarioRepository.DeleteAsync(entity);
            return Result.Ok();
        }

        public async Task<Result<bool>> ValidarPasswordAsync(AutenticarDto dto)
        {
            var validacion = ValidationGeneral.Combinar(
                UsuarioBaseRules.ValidarCorreoInstitucional(dto.Correo),
                ValidationGeneral.Requerido(dto.PasswordHash, "password"));

            if (validacion.EsFallo)
            {
                return Result<bool>.Fallo(validacion.Error!);
            }

            var valido = await _usuarioRepository.ValidarPassword(dto.Correo, dto.PasswordHash);
            return Result<bool>.Ok(valido);
        }

        private static Result<UsuarioTransporte> ConstruirEntidad(CrearUsuarioDto dto)
        {
            UsuarioTransporte entity = dto.TipoUsuario switch
            {
                "Estudiante" => new Estudiante { Matricula = dto.Matricula, Carrera = dto.Carrera },
                "EmpleadoDocente" => new EmpleadoDocente
                {
                    CodigoEmpleado = dto.CodigoEmpleado,
                    Departamento = dto.Departamento,
                    Cargo = dto.Cargo
                },
                "EmpleadoAdministrativo" => new EmpleadoAdministrativo
                {
                    CodigoEmpleado = dto.CodigoEmpleado,
                    Departamento = dto.Departamento,
                    Cargo = dto.Cargo
                },
                "Conductor" => new Conductor { NumeroLicencia = dto.NumeroLicencia, Disponible = dto.Disponible },
                _ => null!
            };

            if (entity is null)
            {
                return Result<UsuarioTransporte>.Fallo(
                    DomainErrors.General.FormatoInvalido(
                        "tipo de usuario",
                        "Estudiante, EmpleadoDocente, EmpleadoAdministrativo o Conductor"));
            }

            entity.Nombre = dto.Nombre;
            entity.Apellido = dto.Apellido;
            entity.Correo = dto.Correo;
            entity.Telefono = dto.Telefono;
            entity.TipoUsuario = dto.TipoUsuario;
            entity.Estado = "Activo";
            entity.RolSistema = dto.RolSistema;
            entity.PasswordHash = dto.PasswordHash;
            entity.FechaCreacion = DateTime.UtcNow;
            entity.CreadoPor = dto.CreadoPor;
            return Result<UsuarioTransporte>.Ok(entity);
        }

        private static Result ValidarUsuario(UsuarioTransporte usuario) => usuario switch
        {
            Estudiante estudiante => EstudianteRules.Validar(estudiante),
            Empleado empleado => EmpleadoRules.Validar(empleado),
            Conductor conductor => ConductorRules.Validar(conductor),
            _ => UsuarioBaseRules.ValidarDatosBase(usuario)
        };

        private static UsuarioTransporte ConvertirAEntidad(UsuarioModel m)
        {
            UsuarioTransporte entity = m switch
            {
                EstudianteModel estudiante => new Estudiante
                {
                    Matricula = estudiante.Matricula,
                    Carrera = estudiante.Carrera,
                    TipoUsuario = "Estudiante"
                },
                EmpleadoModel empleado => new Empleado
                {
                    CodigoEmpleado = empleado.CodigoEmpleado,
                    Departamento = empleado.Departamento,
                    Cargo = empleado.Cargo,
                    TipoUsuario = "Empleado"
                },
                ConductorModel conductor => new Conductor
                {
                    NumeroLicencia = conductor.NumeroLicencia,
                    Disponible = conductor.Disponible,
                    TipoUsuario = "Conductor"
                },
                _ => new Estudiante { TipoUsuario = m.GetType().Name }
            };

            entity.Id = m.Id;
            entity.Nombre = m.Nombre;
            entity.Apellido = m.Apellido;
            entity.Correo = m.Correo;
            entity.Telefono = m.Telefono;
            entity.Estado = m.Estado;
            entity.RolSistema = m.RolSistema;
            return entity;
        }

        private static UsuarioDto Mapear(UsuarioModel m) => m switch
        {
            EstudianteModel e => new(e.Id, e.Nombre, e.Apellido, e.Correo, e.Telefono, e.Estado, "Estudiante", e.RolSistema, e.Matricula, e.Carrera, null, null, null, null, null),
            EmpleadoModel em => new(em.Id, em.Nombre, em.Apellido, em.Correo, em.Telefono, em.Estado, "Empleado", em.RolSistema, null, null, em.CodigoEmpleado, em.Departamento, em.Cargo, null, null),
            ConductorModel c => new(c.Id, c.Nombre, c.Apellido, c.Correo, c.Telefono, c.Estado, "Conductor", c.RolSistema, null, null, null, null, null, c.NumeroLicencia, c.Disponible),
            _ => new(m.Id, m.Nombre, m.Apellido, m.Correo, m.Telefono, m.Estado, m.GetType().Name, m.RolSistema, null, null, null, null, null, null, null)
        };

        private static UsuarioDto MapearEntidad(UsuarioTransporte e) => e switch
        {
            Estudiante est => new(e.Id, e.Nombre, e.Apellido, e.Correo, e.Telefono, e.Estado, "Estudiante", e.RolSistema, est.Matricula, est.Carrera, null, null, null, null, null),
            Empleado em => new(e.Id, e.Nombre, e.Apellido, e.Correo, e.Telefono, e.Estado, e.TipoUsuario ?? "Empleado", e.RolSistema, null, null, em.CodigoEmpleado, em.Departamento, em.Cargo, null, null),
            Conductor c => new(e.Id, e.Nombre, e.Apellido, e.Correo, e.Telefono, e.Estado, "Conductor", e.RolSistema, null, null, null, null, null, c.NumeroLicencia, c.Disponible),
            _ => new(e.Id, e.Nombre, e.Apellido, e.Correo, e.Telefono, e.Estado, e.TipoUsuario ?? "Usuario", e.RolSistema, null, null, null, null, null, null, null)
        };
    }
}
