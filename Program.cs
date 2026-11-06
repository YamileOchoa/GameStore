using Microsoft.EntityFrameworkCore;
using TiendaVideojuegosApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext con PostgreSQL leyendo la cadena de user-secrets
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configurar CORS
var origen = builder.Configuration["Cors:Origin"] ?? "http://localhost:3000";
builder.Services.AddCors(options =>
{
    options.AddPolicy("OrigenPermitido", policy =>
        policy.WithOrigins(origen).AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Dejar Swagger/OpenAPI visible fuera del 'if' de desarrollo para cuando se despliegue
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Tienda Videojuegos API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors("OrigenPermitido");
app.MapControllers();

app.Run();