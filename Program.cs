using LocadoraVeiculos.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LocadoraContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.MapControllers();

app.MapGet("/", () => "API Locadora de Veiculos funcionando!");

app.Run();