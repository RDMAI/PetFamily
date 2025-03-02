using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.PetsManagement.Volunteers.CreateVolunteer;

namespace PetFamily.Application.Shared.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateVolunteerHandler>();
        services.AddScoped<UpdateMainInfoHandler>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
