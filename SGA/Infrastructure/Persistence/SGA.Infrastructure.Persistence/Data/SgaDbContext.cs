using Microsoft.EntityFrameworkCore;
using SGA.Domain.Base;
using SGA.Domain.Entities.Accesos;
using SGA.Domain.Entities.Auditoria;
using SGA.Domain.Entities.Autorizaciones;
using SGA.Domain.Entities.Fotos;
using SGA.Domain.Entities.Notificaciones;
using SGA.Domain.Entities.Pagos;
using SGA.Domain.Entities.Transporte;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Entities.Viajes;

namespace SGA.Infrastructure.Persistence.Data
{
    public sealed class SgaDbContext : DbContext
    {
        public SgaDbContext(DbContextOptions<SgaDbContext> options) : base(options) { }

        public DbSet<UsuarioTransporte> UsuariosTransporte => Set<UsuarioTransporte>();
        public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
        public DbSet<Empleado> Empleados => Set<Empleado>();
        public DbSet<EmpleadoDocente> EmpleadosDocentes => Set<EmpleadoDocente>();
        public DbSet<EmpleadoAdministrativo> EmpleadosAdministrativos => Set<EmpleadoAdministrativo>();
        public DbSet<Conductor> Conductores => Set<Conductor>();

        public DbSet<AutorizacionTransporte> AutorizacionesTransporte => Set<AutorizacionTransporte>();
        public DbSet<TicketDiario> TicketsDiarios => Set<TicketDiario>();
        public DbSet<TarjetaRecargable> TarjetasRecargables => Set<TarjetaRecargable>();
        public DbSet<PermisoTransporte> PermisosTransporte => Set<PermisoTransporte>();
        public DbSet<RecargaTarjeta> RecargasTarjeta => Set<RecargaTarjeta>();

        public DbSet<Ruta> Rutas => Set<Ruta>();
        public DbSet<Parada> Paradas => Set<Parada>();
        public DbSet<HorarioRuta> HorariosRuta => Set<HorarioRuta>();
        public DbSet<Autobus> Autobuses => Set<Autobus>();
        public DbSet<Viaje> Viajes => Set<Viaje>();
        public DbSet<Incidencia> Incidencias => Set<Incidencia>();

        public DbSet<PagoTransporte> PagosTransporte => Set<PagoTransporte>();
        public DbSet<RegistroUsoTransporte> RegistrosUsoTransporte => Set<RegistroUsoTransporte>();
        public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
        public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();
        public DbSet<FotoAutobus> FotosAutobus => Set<FotoAutobus>();
        public DbSet<FotoIncidencia> FotosIncidencia => Set<FotoIncidencia>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioTransporte>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("UsuariosTransporte");
                entity.HasQueryFilter(e => !e.Eliminado);
            });

            modelBuilder.Entity<Estudiante>(entity => { entity.ToTable("Estudiantes"); });
            modelBuilder.Entity<Empleado>(entity => { entity.ToTable("Empleados"); });
            modelBuilder.Entity<EmpleadoDocente>(entity => { entity.ToTable("EmpleadosDocentes"); });
            modelBuilder.Entity<EmpleadoAdministrativo>(entity => { entity.ToTable("EmpleadosAdministrativos"); });
            modelBuilder.Entity<Conductor>(entity => { entity.ToTable("Conductores"); });

            // Rutas
            modelBuilder.Entity<Ruta>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasQueryFilter(e => !e.Eliminado);

                entity.HasMany(e => e.Paradas)
                      .WithOne(p => p.Ruta)
                      .HasForeignKey(p => p.RutaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Horarios)
                      .WithOne(h => h.Ruta)
                      .HasForeignKey(h => h.RutaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Parada>(entity => { entity.HasKey(e => e.Id); });
            modelBuilder.Entity<HorarioRuta>(entity => { entity.HasKey(e => e.Id); });

            // Autobuses
            modelBuilder.Entity<Autobus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasQueryFilter(e => !e.Eliminado);

                entity.HasMany(e => e.Fotos)
                      .WithOne(f => f.Autobus)
                      .HasForeignKey(f => f.AutobusId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Viajes
            modelBuilder.Entity<Viaje>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasQueryFilter(e => !e.Eliminado);

                entity.HasOne(e => e.Ruta).WithMany().HasForeignKey(e => e.RutaId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.HorarioRuta).WithMany().HasForeignKey(e => e.HorarioRutaId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.Autobus).WithMany().HasForeignKey(e => e.AutobusId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.Conductor).WithMany().HasForeignKey(e => e.ConductorId).OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(e => e.Incidencias).WithOne(i => i.Viaje).HasForeignKey(i => i.ViajeId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Incidencia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Conductor).WithMany().HasForeignKey(e => e.ConductorId).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.Fotos).WithOne(f => f.Incidencia).HasForeignKey(f => f.IncidenciaId).OnDelete(DeleteBehavior.Cascade);
            });

            // Autorizaciones y permisos
            modelBuilder.Entity<AutorizacionTransporte>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("AutorizacionesTransporte");
                entity.HasQueryFilter(e => !e.Eliminado);
                entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioTransporteId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<TicketDiario>(entity => { entity.ToTable("TicketsDiarios"); });

            modelBuilder.Entity<TarjetaRecargable>(entity =>
            {
                entity.ToTable("TarjetasRecargables");
                entity.HasMany(e => e.Recargas).WithOne(r => r.TarjetaRecargable).HasForeignKey(r => r.TarjetaRecargableId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PermisoTransporte>(entity => { entity.ToTable("PermisosTransporte"); });
            modelBuilder.Entity<RecargaTarjeta>(entity => { entity.HasKey(e => e.Id); });

            // Pagos
            modelBuilder.Entity<PagoTransporte>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioTransporteId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.RegistradoPor).WithMany().HasForeignKey(e => e.RegistradoPorUsuarioId).OnDelete(DeleteBehavior.NoAction);
            });

            // Accesos
            modelBuilder.Entity<RegistroUsoTransporte>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioTransporteId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.ValidadoPor).WithMany().HasForeignKey(e => e.ValidadoPorUsuarioId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.Viaje).WithMany().HasForeignKey(e => e.ViajeId).OnDelete(DeleteBehavior.NoAction);
            });

            // Notificaciones
            modelBuilder.Entity<Notificacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioTransporteId).OnDelete(DeleteBehavior.NoAction);
            });

            // Auditoria
            modelBuilder.Entity<RegistroAuditoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioTransporteId).OnDelete(DeleteBehavior.NoAction);
            });

            // Fotos
            modelBuilder.Entity<FotoAutobus>(entity => { entity.HasKey(e => e.Id); });
            modelBuilder.Entity<FotoIncidencia>(entity => { entity.HasKey(e => e.Id); });

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var ahora = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<Auditable>())
            {
                if (entry.State == EntityState.Added) entry.Entity.FechaCreacion = ahora;
                else if (entry.State == EntityState.Modified) entry.Entity.FechaModificacion = ahora;
            }
            return await base.SaveChangesAsync(ct);
        }
    }
}
