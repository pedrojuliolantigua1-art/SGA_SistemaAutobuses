using SGA.Domain.Enum;

namespace SGA.Application.DTOs.Usuarios
{
    public sealed record UsuarioDto(
        int Id, string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string Estado, string TipoUsuario, RolUsuario RolSistema,
        string? Matricula, string? Carrera,
        string? CodigoEmpleado, string? Departamento, string? Cargo,
        string? Especialidad, string? TipoContrato, string? AreaAdministrativa,
        string? NumeroLicencia, bool? Disponible);

    public sealed record UsuarioResumenDto(
        int Id, string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string Estado, string TipoUsuario, RolUsuario RolSistema);

    public sealed record ActualizarUsuarioBaseDto(
        int Id, string? Nombre, string? Apellido, string? Correo, string? Telefono, string Estado);

    public sealed record AutenticarDto(string Correo, string PasswordHash);

    public sealed record EstudianteDto(
        int Id, string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string Estado, RolUsuario RolSistema, string? Matricula, string? Carrera);

    public sealed record CrearEstudianteDto(
        string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string? PasswordHash, string? Matricula, string? Carrera, string? CreadoPor);

    public sealed record ActualizarEstudianteDto(
        string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string? Matricula, string? Carrera);

    public sealed record EmpleadoDto(
        int Id, string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string Estado, RolUsuario RolSistema, string? CodigoEmpleado, string? Departamento, string? Cargo);

    public sealed record ActualizarEmpleadoDto(
        int Id, string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string? CodigoEmpleado, string? Departamento, string? Cargo);

    public sealed record EmpleadoDocenteDto(
        int Id, string? Nombre, string? Apellido, string? Correo, string? Telefono, string Estado,
        string? CodigoEmpleado, string? Departamento, string? Cargo, string? Especialidad, string? TipoContrato);

    public sealed record CrearEmpleadoDocenteDto(
        string? Nombre, string? Apellido, string? Correo, string? Telefono, string? PasswordHash,
        string? CodigoEmpleado, string? Departamento, string? Cargo,
        string? Especialidad, string? TipoContrato, string? CreadoPor);

    public sealed record ActualizarEmpleadoDocenteDto(
        string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string? CodigoEmpleado, string? Departamento, string? Cargo, string? Especialidad, string? TipoContrato);

    public sealed record EmpleadoAdministrativoDto(
        int Id, string? Nombre, string? Apellido, string? Correo, string? Telefono, string Estado,
        string? CodigoEmpleado, string? Departamento, string? Cargo, string? AreaAdministrativa);

    public sealed record CrearEmpleadoAdministrativoDto(
        string? Nombre, string? Apellido, string? Correo, string? Telefono, string? PasswordHash,
        string? CodigoEmpleado, string? Departamento, string? Cargo, string? AreaAdministrativa, string? CreadoPor);

    public sealed record ActualizarEmpleadoAdministrativoDto(
        string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string? CodigoEmpleado, string? Departamento, string? Cargo, string? AreaAdministrativa);

    public sealed record ConductorDto(
        int Id, string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string Estado, string? NumeroLicencia, DateTime? FechaVencimientoLicencia, bool Disponible);

    public sealed record CrearConductorDto(
        string? Nombre, string? Apellido, string? Correo, string? Telefono, string? PasswordHash,
        string? NumeroLicencia, DateTime? FechaVencimientoLicencia, string? CreadoPor);

    public sealed record ActualizarConductorDto(
        string? Nombre, string? Apellido, string? Correo, string? Telefono,
        string? NumeroLicencia, DateTime? FechaVencimientoLicencia);

    public sealed record CambiarDisponibilidadConductorDto(bool Disponible);
}
