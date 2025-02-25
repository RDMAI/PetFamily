using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Infrastructure.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDBContext>()
            .AddScoped<IVolunteerRepository, VolunteerRepository>();

        return services;
    }
}
