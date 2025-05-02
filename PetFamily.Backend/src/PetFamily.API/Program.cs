using PetFamily.API.Extensions;
using PetFamily.API.Middlewares;
using PetFamily.Accounts.Presentation;
using PetFamily.Files.Presentation;
using PetFamily.PetsManagement.Presentation;
using PetFamily.SpeciesManagement.Presentation;
using PetFamily.Files.Infrastructure.Extensions;
using Serilog;
using Microsoft.OpenApi.Models;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAPILogging(builder.Configuration)
    .AddAPIServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PetFamily",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insert authentication token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement  // : Dictionary<OpenApiSecurityScheme, IList<string>>
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services
    .AddAccounts(builder.Configuration)
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
    await app.SeedAccounts();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;  // to access Program class in test projects
