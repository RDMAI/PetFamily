﻿using Microsoft.Extensions.DependencyInjection;
using PetFamily.Shared.Core;

namespace PetFamily.SpeciesManagement.Application.Exstensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return DependencyHelper.AddApplicationFromAssembly(
            services,
            assembly: typeof(DependencyInjection).Assembly);
    }
}
