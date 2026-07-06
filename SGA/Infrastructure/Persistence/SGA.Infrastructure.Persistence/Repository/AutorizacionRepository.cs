using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Enum;
using SGA.Domain.Models.Autorizaciones;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Persistence.Data;

namespace SGA.Infrastructure.Persistence.Repositories
{
    public sealed class AutorizacionRepository : IAutorizacionRepository
    {
        private readonly SgaDbContext _context;
        public AutorizacionRepository(SgaDbContext context) => _context = context;

        public async Task<AutorizacionModel?> GetByIdAsync(int id)
        {
            var ticket = await _context.TicketsDiarios.AsNoTracking()
                .Include(t => t.Usuario)
                .Where(t => t.Id == id)
                .Select(t => new TicketDiarioModel
                {
                    Id = t.Id, UsuarioTransporteId = t.UsuarioTransporteId,
                    FechaEmision = t.FechaEmision,
                    Estado = t.Estado,
                    FechaInicio = t.FechaInicio,
                    FechaFin = t.FechaFin,
                    UsuarioNombre = t.Usuario != null ? t.Usuario.Nombre + " " + t.Usuario.Apellido : null
                }).FirstOrDefaultAsync();
            if (ticket is not null) return ticket;

            var tarjeta = await _context.TarjetasRecargables.AsNoTracking()
                .Include(t => t.Usuario)
                .Where(t => t.Id == id)
                .Select(t => new TarjetaRecargableModel
                {
                    Id = t.Id, UsuarioTransporteId = t.UsuarioTransporteId,
                    FechaEmision = t.FechaEmision,
                    Estado = t.Estado,
                    NumeroTarjeta = t.NumeroTarjeta,
                    SaldoDisponible = t.SaldoDisponible,
                    UsuarioNombre = t.Usuario != null ? t.Usuario.Nombre + " " + t.Usuario.Apellido : null
                }).FirstOrDefaultAsync();
            if (tarjeta is not null) return tarjeta;

            var permiso = await _context.PermisosTransporte.AsNoTracking()
                .Include(p => p.Usuario)
                .Where(p => p.Id == id)
                .Select(p => new PermisoTransporteModel
                {
                    Id = p.Id,
                    UsuarioTransporteId = p.UsuarioTransporteId,
                    FechaEmision = p.FechaEmision,
                    Estado = p.Estado,
                    CondicionInstitucional = p.CondicionInstitucional,
                    FechaVencimiento = p.FechaVencimiento,
                    UsuarioNombre = p.Usuario != null ? p.Usuario.Nombre + " " + p.Usuario.Apellido : null
                }).FirstOrDefaultAsync();

            return permiso;
        }

        public async Task<IReadOnlyList<AutorizacionModel>> GetAllAsync()
        {
            var tickets = await _context.TicketsDiarios.AsNoTracking()
                .Include(t => t.Usuario)
                .Select(t => new TicketDiarioModel
                {
                    Id = t.Id, UsuarioTransporteId = t.UsuarioTransporteId,
                    FechaEmision = t.FechaEmision,
                    Estado = t.Estado,
                    FechaInicio = t.FechaInicio,
                    FechaFin = t.FechaFin,
                    UsuarioNombre = t.Usuario != null ? t.Usuario.Nombre + " " + t.Usuario.Apellido : null
                }).ToListAsync();

            var tarjetas = await _context.TarjetasRecargables.AsNoTracking()
                .Include(t => t.Usuario)
                .Select(t => new TarjetaRecargableModel
                {
                    Id = t.Id, UsuarioTransporteId = t.UsuarioTransporteId,
                    FechaEmision = t.FechaEmision, Estado = t.Estado,
                    NumeroTarjeta = t.NumeroTarjeta, SaldoDisponible = t.SaldoDisponible,
                    UsuarioNombre = t.Usuario != null ? t.Usuario.Nombre + " " + t.Usuario.Apellido : null
                }).ToListAsync();

            var permisos = await _context.PermisosTransporte.AsNoTracking()
                .Include(p => p.Usuario)
                .Select(p => new PermisoTransporteModel
                {
                    Id = p.Id, UsuarioTransporteId = p.UsuarioTransporteId,
                    FechaEmision = p.FechaEmision, Estado = p.Estado,
                    CondicionInstitucional = p.CondicionInstitucional, FechaVencimiento = p.FechaVencimiento,
                    UsuarioNombre = p.Usuario != null ? p.Usuario.Nombre + " " + p.Usuario.Apellido : null
                }).ToListAsync();

            return tickets.Cast<AutorizacionModel>().Concat(tarjetas).Concat(permisos).ToList();
        }

        public async Task<AutorizacionModel> GetbyUsuario(int usuarioId)
        {
            var id = await _context.AutorizacionesTransporte.AsNoTracking()
                .Where(a => a.UsuarioTransporteId == usuarioId && a.Estado == EstadoAutorizacion.Activa)
                .OrderByDescending(a => a.FechaEmision)
                .Select(a => a.Id)
                .FirstAsync();
            return (await GetByIdAsync(id))!;
        }

        public async Task<IReadOnlyList<AutorizacionModel>> GetVigentes()
        {
            var ids = await _context.AutorizacionesTransporte.AsNoTracking()
                .Where(a => a.Estado == EstadoAutorizacion.Activa)
                .Select(a => a.Id)
                .ToListAsync();
            return (await GetAllAsync()).Where(a => ids.Contains(a.Id)).ToList();
        }

        public async Task<IReadOnlyList<AutorizacionModel>> GetbyPeriodo(DateTime desde, DateTime hasta)
        {
            var ids = await _context.AutorizacionesTransporte.AsNoTracking()
                .Where(a => a.FechaEmision >= desde && a.FechaEmision <= hasta)
                .Select(a => a.Id)
                .ToListAsync();
            return (await GetAllAsync()).Where(a => ids.Contains(a.Id)).ToList();
        }

        public async Task AddAsync(AutorizacionTransporte entity) { await _context.AddAsync(entity); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(AutorizacionTransporte entity) { _context.Update(entity); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(AutorizacionTransporte entity)
        {
            entity.Eliminado = true;
            entity.FechaEliminacion = DateTime.UtcNow;
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<(int PagoId, int AutorizacionId)> EmitirAutorizacionAsync(
            int usuarioId, decimal monto, string tipoPago, string numeroComprobante,
            DateTime fechaHora, int registradoPorId, string tipoAutorizacion, DateTime fechaEmision,
            DateTime? fechaInicio, DateTime? fechaFin, string? numeroTarjeta, decimal? saldoInicial,
            string? condicionInstitucional, DateTime? fechaVencimiento, string creadoPor)
        {
            await using var transaccion = await _context.Database.BeginTransactionAsync();

            AutorizacionTransporte autorizacion = tipoAutorizacion switch
            {
                "TicketDiario" => new TicketDiario
                {
                    UsuarioTransporteId = usuarioId, FechaEmision = fechaEmision,
                    FechaInicio = fechaInicio!.Value, FechaFin = fechaFin!.Value, CreadoPor = creadoPor
                },
                "TarjetaRecargable" => new TarjetaRecargable
                {
                    UsuarioTransporteId = usuarioId, FechaEmision = fechaEmision,
                    NumeroTarjeta = numeroTarjeta, SaldoDisponible = saldoInicial ?? 0, CreadoPor = creadoPor
                },
                "PermisoTransporte" => new PermisoTransporte
                {
                    UsuarioTransporteId = usuarioId, FechaEmision = fechaEmision,
                    CondicionInstitucional = condicionInstitucional, FechaVencimiento = fechaVencimiento, CreadoPor = creadoPor
                },
                _ => throw new InvalidOperationException($"Tipo de autorizacion no reconocido: {tipoAutorizacion}")
            };

            await _context.AddAsync(autorizacion);
            await _context.SaveChangesAsync();

            var pago = new PagoTransporte
            {
                UsuarioTransporteId = usuarioId, AutorizacionTransporteId = autorizacion.Id,
                Monto = monto, TipoPago = tipoPago, Estado = "Confirmado",
                NumeroComprobante = numeroComprobante, FechaHora = fechaHora,
                RegistradoPorUsuarioId = registradoPorId, CreadoPor = creadoPor
            };
            await _context.PagosTransporte.AddAsync(pago);
            await _context.SaveChangesAsync();

            await transaccion.CommitAsync();
            return (pago.Id, autorizacion.Id);
        }
    }
}
