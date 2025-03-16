using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.PetsManagement.Pets.AddPet;
using PetFamily.Application.PetsManagement.Pets.MovePet;
using PetFamily.Application.PetsManagement.Pets.UploadPetPhotos;
using PetFamily.Application.PetsManagement.Volunteers.CreateVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.DeleteVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.UpdateMainInfo;
using PetFamily.Application.PetsManagement.Volunteers.UpdateRequisites;
using PetFamily.Application.PetsManagement.Volunteers.UpdateSocialNetworks;

namespace PetFamily.Application.Shared.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateVolunteerHandler>();
        services.AddScoped<UpdateMainInfoHandler>();
        services.AddScoped<UpdateSocialNetworksHandler>();
        services.AddScoped<UpdateRequisitesHandler>();
        services.AddScoped<DeleteVolunteerHandler>();

        services.AddScoped<AddPetHandler>();
        services.AddScoped<MovePetHandler>();
        services.AddScoped<UploadPetPhotosHandler>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
