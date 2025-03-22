using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.PetsManagement.Pets.Commands.AddPet;
using PetFamily.Application.PetsManagement.Pets.Commands.DeletePetPhotos;
using PetFamily.Application.PetsManagement.Pets.Commands.MovePet;
using PetFamily.Application.PetsManagement.Pets.Commands.UploadPetPhotos;
using PetFamily.Application.PetsManagement.Pets.UploadPetPhotos;
using PetFamily.Application.PetsManagement.Volunteers.Commands.CreateVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.Commands.DeleteVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateMainInfo;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateRequisites;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;

namespace PetFamily.Application.Shared.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddCommands()
            .AddQueries();

        services
            .AddScoped<GetPetPhotosHandler>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }

    private static IServiceCollection AddCommands(this IServiceCollection services)
    {
        return services.Scan(scan => scan
            .FromAssemblies(typeof(DependencyInjection).Assembly)
                .AddClasses(classes => classes.AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());
    }

    private static IServiceCollection AddQueries(this IServiceCollection services)
    {
        return services.Scan(scan => scan
            .FromAssemblies(typeof(DependencyInjection).Assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());
    }
}
