using Microsoft.Data.SqlClient;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Enum;
using SGA.Domain.Models.Usuarios;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Abstractions;
using SGA.Infrastructure.Persistence.Common;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class UsuarioRepository : SqlRepositoryBase, IUsuarioRepository
    {
        public UsuarioRepository(ISqlConnectionfactory factory) : base(factory) { }

        public async Task<UsuarioModel?> GetByIdAsync(int id)
            => await QuerySingleOrDefaultAsync("sp_Usuario_GetById", UsuarioMapper.Map, Param("@Id", id));

        public async Task<IReadOnlyList<UsuarioModel>> GetAllAsync()
            => await QueryAsync("sp_Usuario_GetAll", UsuarioMapper.Map);

        public async Task AddAsync(UsuarioTransporte entity)
            => entity.Id = await ExecuteScalarAsync("sp_Usuario_Insert", UsuarioParameters.ParaInsertar(entity));

        public async Task UpdateAsync(UsuarioTransporte entity)
            => await ExecuteAsync("sp_Usuario_Update", UsuarioParameters.ParaActualizar(entity));

        public async Task DeleteAsync(UsuarioTransporte entity)
            => await ExecuteAsync("sp_Usuario_Delete", UsuarioParameters.ParaEliminar(entity));

        public async Task<UsuarioModel?> GetbyCorreo(string correo)
            => await QuerySingleOrDefaultAsync("sp_Usuario_GetByCorreo", UsuarioMapper.Map, Param("@Correo", correo));

        public async Task<UsuarioModel?> GetbyRol(RolUsuario rol)
            => await QuerySingleOrDefaultAsync("sp_Usuario_GetByRol", UsuarioMapper.Map, Param("@RolSistema", (int)rol));

        public async Task<bool> ValidarPassword(string correo, string passwordHash)
        {
            var count = await ExecuteScalarAsync("sp_Usuario_ValidarPassword",
                Param("@Correo", correo),
                Param("@PasswordHash", passwordHash));
            return count > 0;
        }
    }

    internal static class UsuarioMapper
    {
        internal static UsuarioModel Map(SqlReaderRow r)
        {
            var tipo = r.Str("TipoUsuario");

            UsuarioModel model = tipo switch
            {
                "Estudiante" => new EstudianteModel
                {
                    Matricula = r.Str("Matricula"),
                    Carrera = r.Str("Carrera")
                },
                "EmpleadoDocente" => new EmpleadoModel
                {
                    CodigoEmpleado = r.Str("CodigoEmpleado"),
                    Departamento = r.Str("Departamento"),
                    Cargo = r.Str("Cargo")
                },
                "EmpleadoAdministrativo" => new EmpleadoModel
                {
                    CodigoEmpleado = r.Str("CodigoEmpleado"),
                    Departamento = r.Str("Departamento"),
                    Cargo = r.Str("Cargo")
                },
                "Conductor" => new ConductorModel
                {
                    NumeroLicencia = r.Str("NumeroLicencia"),
                    Disponible = r.Bool("Disponible")
                },
                _ => throw new InvalidOperationException($"TipoUsuario desconocido: {tipo}")
            };

                model.Id = r.Int("Id");
                model.Nombre = r.Str("Nombre");
                model.Apellido = r.Str("Apellido");
                model.Correo = r.Str("Correo");
                model.Telefono = r.Str("Telefono");
                model.Estado = r.Str("Estado") ?? "Activo";
                model.RolSistema = r.Enum<RolUsuario>("RolSistema");
                return model;
        }
    }

    internal static class UsuarioParameters
    {
        internal static SqlParameter[] ParaInsertar(UsuarioTransporte u) =>
        [
            SqlRepositoryBase.Param("@Nombre",             u.Nombre),
            SqlRepositoryBase.Param("@Apellido",           u.Apellido),
            SqlRepositoryBase.Param("@Correo",             u.Correo),
            SqlRepositoryBase.Param("@Telefono",           u.Telefono),
            SqlRepositoryBase.Param("@TipoUsuario",        u.TipoUsuario),
            SqlRepositoryBase.Param("@Estado",             u.Estado),
            SqlRepositoryBase.Param("@RolSistema",         (int)u.RolSistema),
            SqlRepositoryBase.Param("@Matricula",          (u as Estudiante)?.Matricula),
            SqlRepositoryBase.Param("@Carrera",            (u as Estudiante)?.Carrera),
            SqlRepositoryBase.Param("@CodigoEmpleado",     (u as Empleado)?.CodigoEmpleado),
            SqlRepositoryBase.Param("@Departamento",       (u as Empleado)?.Departamento),
            SqlRepositoryBase.Param("@Cargo",              (u as Empleado)?.Cargo),
            SqlRepositoryBase.Param("@NumeroLicencia",     (u as Conductor)?.NumeroLicencia),
            SqlRepositoryBase.Param("@Disponible",         (u as Conductor)?.Disponible ?? false),
            SqlRepositoryBase.Param("@CreadoPor",          u.CreadoPor)
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