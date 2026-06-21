using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Enum;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class UsuarioRepository : SqlRepositoryBase, IUsuarioRepository
    {
        public UsuarioRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<UsuarioTransporte?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync(
                "sp_Usuario_GetById", UsuarioMapper.Map,
                Param("@Id", id));

        public async Task<IReadOnlyList<UsuarioTransporte>> GetAllAsync()
            => await QueryAsync("sp_Usuario_GetAll", UsuarioMapper.Map);

        public async Task AddAsync(UsuarioTransporte entity)
            => entity.Id = await ExecuteScalarAsync(
                "sp_Usuario_Insert",
                UsuarioParameters.ParaInsertar(entity));

        public async Task UpdateAsync(UsuarioTransporte entity)
            => await ExecuteAsync("sp_Usuario_Update",
                UsuarioParameters.ParaActualizar(entity));

        public async Task DeleteAsync(UsuarioTransporte entity)
            => await ExecuteAsync("sp_Usuario_Delete",
                UsuarioParameters.ParaEliminar(entity));

        public async Task<UsuarioTransporte?> Getby_Correo(string correo)
            => await QuerySingleOrDefaultAsync(
                "sp_Usuario_GetByCorreo", UsuarioMapper.Map,
                Param("@Correo", correo));

        public async Task<UsuarioTransporte> Getby_Rol(RolUsuario rol)
        {
            var usuario = await QuerySingleOrDefaultAsync(
                "sp_Usuario_GetByRol", UsuarioMapper.Map,
                Param("@RolSistema", (int)rol));

            return usuario ?? throw new InvalidOperationException(
                $"No se encontro usuario con rol {rol}.");
        }

        public async Task<bool> Validar_Password(string correo, string passwordHash)
        {
            var count = await ExecuteScalarAsync(
                "sp_Usuario_ValidarPassword",
                Param("@Correo", correo),
                Param("@PasswordHash", passwordHash));

            return count > 0;
        }
    }

    internal static class UsuarioMapper
    {
        internal static UsuarioTransporte Map(SqlDataReader r)
        {
            var tipo = SqlRepositoryBase.GetString(r, "TipoUsuario");

            UsuarioTransporte u = tipo switch
            {
                "Estudiante" => MapEstudiante(r),
                "EmpleadoDocente" => MapEmpleadoDocente(r),
                "EmpleadoAdministrativo" => MapEmpleadoAdministrativo(r),
                "Conductor" => MapConductor(r),
                _ => throw new InvalidOperationException(
                         $"TipoUsuario desconocido: {tipo}")
            };

            MapCamposComunes(r, u, tipo);
            return u;
        }

        private static void MapCamposComunes(
            SqlDataReader r, UsuarioTransporte u, string? tipo)
        {
            u.Id = SqlRepositoryBase.GetInt(r, "Id");
            u.CodigoInstitucional = SqlRepositoryBase.GetString(r, "CodigoInstitucional");
            u.Nombre = SqlRepositoryBase.GetString(r, "Nombre");
            u.Apellido = SqlRepositoryBase.GetString(r, "Apellido");
            u.Correo = SqlRepositoryBase.GetString(r, "Correo");
            u.Telefono = SqlRepositoryBase.GetString(r, "Telefono");
            u.TipoUsuario = tipo;
            u.Estado = SqlRepositoryBase.GetString(r, "Estado") ?? "Activo";
            u.RolSistema = SqlRepositoryBase.GetEnum<RolUsuario>(r, "RolSistema");
            u.FechaCreacion = SqlRepositoryBase.GetDateTime(r, "FechaCreacion");
            u.CreadoPor = SqlRepositoryBase.GetString(r, "CreadoPor");
            u.FechaModificacion = SqlRepositoryBase.GetDateTime(r, "FechaModificacion");
            u.Eliminado = SqlRepositoryBase.GetBool(r, "Eliminado");
            u.EliminadoPor = SqlRepositoryBase.GetString(r, "EliminadoPor");
        }

        private static Estudiante MapEstudiante(SqlDataReader r) => new()
        {
            Matricula = SqlRepositoryBase.GetString(r, "Matricula"),
            Carrera = SqlRepositoryBase.GetString(r, "Carrera")
        };

        private static EmpleadoDocente MapEmpleadoDocente(SqlDataReader r) => new()
        {
            CodigoEmpleado = SqlRepositoryBase.GetString(r, "CodigoEmpleado"),
            Departamento = SqlRepositoryBase.GetString(r, "Departamento"),
            Cargo = SqlRepositoryBase.GetString(r, "Cargo"),
            Especialidad = SqlRepositoryBase.GetString(r, "Especialidad"),
            TipoContrato = SqlRepositoryBase.GetString(r, "TipoContrato")
        };

        private static EmpleadoAdministrativo MapEmpleadoAdministrativo(SqlDataReader r) => new()
        {
            CodigoEmpleado = SqlRepositoryBase.GetString(r, "CodigoEmpleado"),
            Departamento = SqlRepositoryBase.GetString(r, "Departamento"),
            Cargo = SqlRepositoryBase.GetString(r, "Cargo"),
            AreaAdministrativa = SqlRepositoryBase.GetString(r, "AreaAdministrativa") ?? string.Empty
        };

        private static Conductor MapConductor(SqlDataReader r) => new()
        {
            NumeroLicencia = SqlRepositoryBase.GetString(r, "NumeroLicencia"),
            Disponible = SqlRepositoryBase.GetBool(r, "Disponible")
        };
    }

    internal static class UsuarioParameters
    {
        internal static SqlParameter[] ParaInsertar(UsuarioTransporte u) =>
        [
            SqlRepositoryBase.Param("@CodigoInstitucional", u.CodigoInstitucional),
            SqlRepositoryBase.Param("@Nombre",              u.Nombre),
            SqlRepositoryBase.Param("@Apellido",            u.Apellido),
            SqlRepositoryBase.Param("@Correo",              u.Correo),
            SqlRepositoryBase.Param("@Telefono",            u.Telefono),
            SqlRepositoryBase.Param("@TipoUsuario",         u.TipoUsuario),
            SqlRepositoryBase.Param("@Estado",              u.Estado),
            SqlRepositoryBase.Param("@RolSistema",          (int)u.RolSistema),
            SqlRepositoryBase.Param("@Matricula",           (u as Estudiante)?.Matricula),
            SqlRepositoryBase.Param("@Carrera",             (u as Estudiante)?.Carrera),
            SqlRepositoryBase.Param("@CodigoEmpleado",      (u as Empleado)?.CodigoEmpleado),
            SqlRepositoryBase.Param("@Departamento",        (u as Empleado)?.Departamento),
            SqlRepositoryBase.Param("@Cargo",               (u as Empleado)?.Cargo),
            SqlRepositoryBase.Param("@Especialidad",        (u as EmpleadoDocente)?.Especialidad),
            SqlRepositoryBase.Param("@TipoContrato",        (u as EmpleadoDocente)?.TipoContrato),
            SqlRepositoryBase.Param("@AreaAdministrativa",  (u as EmpleadoAdministrativo)?.AreaAdministrativa),
            SqlRepositoryBase.Param("@NumeroLicencia",      (u as Conductor)?.NumeroLicencia),
            SqlRepositoryBase.Param("@Disponible",          (u as Conductor)?.Disponible ?? false),
            SqlRepositoryBase.Param("@CreadoPor",           u.CreadoPor)
        ];

        internal static SqlParameter[] ParaActualizar(UsuarioTransporte u) =>
        [
            SqlRepositoryBase.Param("@Id", u.Id),
            ..ParaInsertar(u)
        ];

        internal static SqlParameter[] ParaEliminar(UsuarioTransporte u) =>
        [
            SqlRepositoryBase.Param("@Id",           u.Id),
            SqlRepositoryBase.Param("@EliminadoPor", u.EliminadoPor)
        ];
    }
}
