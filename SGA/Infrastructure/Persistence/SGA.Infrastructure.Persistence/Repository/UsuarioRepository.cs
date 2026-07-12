using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Enum;
using SGA.Domain.Models.Usuarios;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Data;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class UsuarioRepository : IUsuarioRepository
    {
        private readonly SgaDbContext _context;
        public UsuarioRepository(SgaDbContext context) => _context = context;

        public async Task<IReadOnlyList<UsuarioModel>> GetAllAsync()
        {
            var estudiantes = await _context.Estudiantes.AsNoTracking()
                .Select(e => new EstudianteModel
                {
                    Id = e.Id,
                    Nombre = e.Nombre, 
                    Apellido = e.Apellido,
                    Correo = e.Correo,
                    Telefono = e.Telefono,
                    Estado = e.Estado,
                    RolSistema = e.RolSistema,
                    Matricula = e.Matricula,
                    Carrera = e.Carrera
                }).ToListAsync();

            var conductores = await _context.Conductores.AsNoTracking()
                .Select(c => new ConductorModel
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    Correo = c.Correo,
                    Telefono = c.Telefono,
                    Estado = c.Estado,
                    RolSistema = c.RolSistema,
                    NumeroLicencia = c.NumeroLicencia,
                    FechaVencimientoLicencia = c.FechaVencimientoLicencia,
                    Disponible = c.Disponible
                }).ToListAsync();

            var docentes = await _context.EmpleadosDocentes.AsNoTracking()
                .Select(d => new EmpleadoDocenteModel
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    Apellido = d.Apellido,
                    Correo = d.Correo,
                    Telefono = d.Telefono,
                    Estado = d.Estado, 
                    RolSistema = d.RolSistema,
                    CodigoEmpleado = d.CodigoEmpleado,
                    Departamento = d.Departamento,
                    Cargo = d.Cargo,
                    Especialidad = d.Especialidad,
                    TipoContrato = d.TipoContrato
                }).ToListAsync();

            var administrativos = await _context.EmpleadosAdministrativos.AsNoTracking()
                .Select(a => new EmpleadoAdministrativoModel
                {
                    Id = a.Id,
                    Nombre = a.Nombre, 
                    Apellido = a.Apellido,
                    Correo = a.Correo,
                    Telefono = a.Telefono,
                    Estado = a.Estado,
                    RolSistema = a.RolSistema,
                    CodigoEmpleado = a.CodigoEmpleado,
                    Departamento = a.Departamento,
                    Cargo = a.Cargo,
                    AreaAdministrativa = a.AreaAdministrativa
                }).ToListAsync();

            return estudiantes.Cast<UsuarioModel>()
                .Concat(conductores)
                .Concat(docentes)
                .Concat(administrativos)
                .ToList();
        }

        public async Task<UsuarioModel?> GetByIdAsync(int id)
        {
            var estudiante = await _context.Estudiantes.AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EstudianteModel
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Correo = e.Correo,
                    Telefono = e.Telefono,
                    Estado = e.Estado,
                    RolSistema = e.RolSistema,
                    Matricula = e.Matricula,
                    Carrera = e.Carrera
                }).FirstOrDefaultAsync();
            if (estudiante is not null) return estudiante;

            var conductor = await _context.Conductores.AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new ConductorModel
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    Correo = c.Correo,
                    Telefono = c.Telefono,
                    Estado = c.Estado,
                    RolSistema = c.RolSistema,
                    NumeroLicencia = c.NumeroLicencia,
                    FechaVencimientoLicencia = c.FechaVencimientoLicencia,
                    Disponible = c.Disponible
                }).FirstOrDefaultAsync();
            if (conductor is not null) return conductor;

            var docente = await _context.EmpleadosDocentes.AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new EmpleadoDocenteModel
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    Apellido = d.Apellido,
                    Correo = d.Correo,
                    Telefono = d.Telefono,
                    Estado = d.Estado,
                    RolSistema = d.RolSistema,
                    CodigoEmpleado = d.CodigoEmpleado,
                    Departamento = d.Departamento,
                    Cargo = d.Cargo,
                    Especialidad = d.Especialidad,
                    TipoContrato = d.TipoContrato
                }).FirstOrDefaultAsync();
            if (docente is not null) return docente;

            var administrativo = await _context.EmpleadosAdministrativos.AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new EmpleadoAdministrativoModel
                {
                    Id = a.Id, Nombre = a.Nombre, Apellido = a.Apellido, Correo = a.Correo,
                    Telefono = a.Telefono, Estado = a.Estado, RolSistema = a.RolSistema,
                    CodigoEmpleado = a.CodigoEmpleado, Departamento = a.Departamento, Cargo = a.Cargo,
                    AreaAdministrativa = a.AreaAdministrativa
                }).FirstOrDefaultAsync();

            return administrativo;
        }

        public async Task<UsuarioModel?> GetbyCorreo(string correo)
        {
            var baseUsuario = await _context.UsuariosTransporte.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Correo == correo);
            return baseUsuario is null ? null : await GetByIdAsync(baseUsuario.Id);
        }

        public async Task<UsuarioModel> GetbyRol(RolUsuario rol)
        {
            var id = await _context.UsuariosTransporte.AsNoTracking()
                .Where(u => u.RolSistema == rol)
                .Select(u => u.Id)
                .FirstAsync();
            return (await GetByIdAsync(id))!;
        }

        public async Task<bool> ValidarPassword(string correo, string passwordHash) =>
            await _context.UsuariosTransporte.AsNoTracking()
                .AnyAsync(u => u.Correo == correo && u.PasswordHash == passwordHash);

        public async Task<UsuarioModel?> GetByMatricula(string matricula) =>
            await _context.Estudiantes.AsNoTracking()
                .Where(e => e.Matricula == matricula)
                .Select(e => new EstudianteModel
                {
                    Id = e.Id, Nombre = e.Nombre, Apellido = e.Apellido, Correo = e.Correo,
                    Telefono = e.Telefono, Estado = e.Estado, RolSistema = e.RolSistema,
                    Matricula = e.Matricula, Carrera = e.Carrera
                }).FirstOrDefaultAsync();

        public async Task<UsuarioModel?> GetByNumeroLicencia(string numeroLicencia) =>
            await _context.Conductores.AsNoTracking()
                .Where(c => c.NumeroLicencia == numeroLicencia)
                .Select(c => new ConductorModel
                {
                    Id = c.Id, Nombre = c.Nombre, Apellido = c.Apellido, Correo = c.Correo,
                    Telefono = c.Telefono, Estado = c.Estado, RolSistema = c.RolSistema,
                    NumeroLicencia = c.NumeroLicencia,
                    FechaVencimientoLicencia = c.FechaVencimientoLicencia,
                    Disponible = c.Disponible
                }).FirstOrDefaultAsync();

        public async Task<UsuarioModel?> GetByCodigoEmpleado(string codigoEmpleado)
        {
            var docente = await _context.EmpleadosDocentes.AsNoTracking()
                .Where(d => d.CodigoEmpleado == codigoEmpleado)
                .Select(d => new EmpleadoDocenteModel
                {
                    Id = d.Id, Nombre = d.Nombre, Apellido = d.Apellido, Correo = d.Correo,
                    Telefono = d.Telefono, Estado = d.Estado, RolSistema = d.RolSistema,
                    CodigoEmpleado = d.CodigoEmpleado, Departamento = d.Departamento, Cargo = d.Cargo,
                    Especialidad = d.Especialidad, TipoContrato = d.TipoContrato
                }).FirstOrDefaultAsync();
            if (docente is not null) return docente;

            return await _context.EmpleadosAdministrativos.AsNoTracking()
                .Where(a => a.CodigoEmpleado == codigoEmpleado)
                .Select(a => new EmpleadoAdministrativoModel
                {
                    Id = a.Id, Nombre = a.Nombre, Apellido = a.Apellido, Correo = a.Correo,
                    Telefono = a.Telefono, Estado = a.Estado, RolSistema = a.RolSistema,
                    CodigoEmpleado = a.CodigoEmpleado, Departamento = a.Departamento, Cargo = a.Cargo,
                    AreaAdministrativa = a.AreaAdministrativa
                }).FirstOrDefaultAsync();
        }

        public async Task AddAsync(UsuarioTransporte entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UsuarioTransporte entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UsuarioTransporte entity)
        {
            entity.Eliminado = true;
            entity.FechaEliminacion = DateTime.UtcNow;
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
