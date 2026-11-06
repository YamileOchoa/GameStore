using Microsoft.EntityFrameworkCore;
using TiendaVideojuegosApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext con SQL Server leyendo la cadena de conexión
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configurar CORS
var origen = builder.Configuration["Cors:Origin"] ?? "*";
builder.Services.AddCors(options =>
{
    options.AddPolicy("OrigenPermitido", policy =>
        policy.WithOrigins(origen).AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Swagger/OpenAPI visible en cualquier entorno (desarrollo y producción)
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Tienda Videojuegos API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors("OrigenPermitido");
app.MapControllers();

app.Run();