using Microsoft.OpenApi.Models;
using SGA.Application;
using SGA.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Política CORS para permitir el acceso de los clientes Web y Desktop.
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

// Configuración de Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SGA API - Sistema de Gestion de Autorizaciones de Transporte",
        Version = "v1"
    });
});

var app = builder.Build();

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
