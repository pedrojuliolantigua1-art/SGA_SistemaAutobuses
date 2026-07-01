
using SGA.Domain.Enum;

namespace SGA.Application.DTOs.Usuarios
{
    public sealed record UsuarioDto(
        int Id,
        string? Nombre,
        string? Apellido,
        string? Correo,
        string? Telefono,
        string Estado,
        string TipoUsuario,
        RolUsuario RolSistema,
        string? Matricula,
        string? Carrera,
        string? CodigoEmpleado,
        string? Departamento,
        string? Cargo,
        string? NumeroLicencia,
        bool? Disponible);

    public sealed record CrearUsuarioDto(
        string? Nombre,
        string? Apellido,
        string? Correo,
        string? Telefono,
        string TipoUsuario,
        RolUsuario RolSistema,
        string? PasswordHash,
        string? CreadoPor,
        string? Matricula = null,
        string? Carrera = null,
        string? CodigoEmpleado = null,
        string? Departamento = null,
        string? Cargo = null,
        string? NumeroLicencia = null,
        bool Disponible = false);

    public sealed record ActualizarUsuarioDto(
        int Id,
        string? Nombre,
        string? Apellido,
        string? Correo,
        string? Telefono,
        string Estado);

    public sealed record AutenticarDto(
        string Correo,
        string PasswordHash);
}