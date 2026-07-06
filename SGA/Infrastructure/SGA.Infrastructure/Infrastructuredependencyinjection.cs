using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGA.Domain.Repository.Interfaces;
using SGA.Infrastructure.Email;
using SGA.Infrastructure.Almacenamiento;
using SGA.Infrastructure.Persistence.Data;
using SGA.Infrastructure.Persistence.Repositories;
using SGA.Domain.Services;

namespace SGA.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddPersistence(configuration);
            services.AddEmail(configuration);
            services.AddAlmacenamiento(configuration);

            return services;
        }

        //persistencia con entity framework core
        private static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection no esta configurado en appsettings.json");

            services.AddDbContext<SgaDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IViajeRepository, ViajeRepository>();
            services.AddScoped<IAutorizacionRepository, AutorizacionRepository>();
            services.AddScoped<IAccesoRepository, AccesoRepository>();
            services.AddScoped<IPagoRepository, PagoRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.AddScoped<INotificacionRepository, NotificacionRepository>();
            services.AddScoped<IRutaRepository, RutaRepository>();
            services.AddScoped<IAutobusRepository, AutobusRepository>();
            services.AddScoped<IParadaRepository, ParadaRepository>();
            services.AddScoped<IHorarioRutaRepository, HorarioRutaRepository>();
            services.AddScoped<IFotoAutobusRepository, FotoAutobusRepository>();
            services.AddScoped<IFotoIncidenciaRepository, FotoIncidenciaRepository>();

            return services;
        }

        // email
        private static IServiceCollection AddEmail(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<SmtpOptions>(
                configuration.GetSection("Email:Smtp"));

            services.AddTransient<IEmailSender, SmtpEmailSender>();

            return services;
        }

        // para las imagenes con cludinary
        private static IServiceCollection AddAlmacenamiento(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<CloudinaryOptions>(
                configuration.GetSection("Cloudinary"));

            services.AddSingleton<IAlmacenamientoArchivos, CloudinaryService>();

            return services;
        }
    }
}
