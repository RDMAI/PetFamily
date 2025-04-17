using PetFamily.API.Extensions;
using PetFamily.API.Middlewares;
using PetFamily.Files.Presentation;
using PetFamily.PetsManagement.Presentation;
using PetFamily.SpeciesManagement.Presentation;
using PetFamily.Files.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAPILogging(builder.Configuration)
    .AddAPIServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddFilesManagement(builder.Configuration)
    .AddPetsManagement(builder.Configuration)
    .AddSpeciesManagement(builder.Configuration);

var app = builder.Build();

app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyModulesMigrations();
    await app.CreateFileStorageStructure();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program;  // to access Program class in test projects
