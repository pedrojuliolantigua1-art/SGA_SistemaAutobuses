using Microsoft.OpenApi.Models;
using SGA.Application;
using SGA.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---------- Servicios de la aplicacion ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ---------- CORS ----------
// Los clientes Web/Desktop consumen esta Api desde otro origen.
// En produccion, reemplazar AllowAnyOrigin por la lista de dominios reales.
const string PoliticaClientesSga = "SgaClients";

builder.Services.AddCors(options =>
{
    options.AddPolicy(PoliticaClientesSga, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------- Swagger ----------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SGA API - Sistema de Gestion de Autorizaciones de Transporte",
        Version = "v1"
    });
});

var app = builder.Build();

// ---------- Pipeline HTTP ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SGA API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors(PoliticaClientesSga);

app.MapControllers();

app.Run();
