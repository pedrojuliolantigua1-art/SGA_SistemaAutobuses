using Microsoft.Extensions.DependencyInjection;
using SGA.Application.Interfaces.Services;
using SGA.Application.Services;

namespace SGA.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IRutaService, RutaService>();
            services.AddScoped<IParadaService, ParadaService>();
            services.AddScoped<IHorarioRutaService, HorarioRutaService>();
            services.AddScoped<IAutobusService, AutobusService>();
            services.AddScoped<IViajeService, ViajeService>();
            services.AddScoped<IAutorizacionService, AutorizacionService>();
            services.AddScoped<IAccesoService, AccesoService>();
            services.AddScoped<IPagoService, PagoService>();
            services.AddScoped<INotificacionService, NotificacionService>();
            services.AddScoped<IAuditoriaService, AuditoriaService>();

            return services;
        }
    }
}
