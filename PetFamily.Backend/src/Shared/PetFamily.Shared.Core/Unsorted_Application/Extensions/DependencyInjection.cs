//using FluentValidation;
//using Microsoft.Extensions.DependencyInjection;
//using PetFamily.Application.PetsManagement.Pets.Queries.GetPetPhotos;
//using PetFamily.Application.Shared.Abstractions;

//namespace PetFamily.Shared.Core.Unsorted_Application.Extensions;
//public static class DependencyInjection
//{
//    public static IServiceCollection AddApplication(this IServiceCollection services)
//    {
//        services
//            .AddCommands()
//            .AddQueries();

//        services
//            .AddScoped<GetPetPhotosHandler>();

//        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

//        return services;
//    }

//    private static IServiceCollection AddCommands(this IServiceCollection services)
//    {
//        return services.Scan(scan => scan
//            .FromAssemblies(typeof(DependencyInjection).Assembly)
//                .AddClasses(classes => classes.AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
//            .AsSelfWithInterfaces()
//            .WithScopedLifetime());
//    }

//    private static IServiceCollection AddQueries(this IServiceCollection services)
//    {
//        return services.Scan(scan => scan
//            .FromAssemblies(typeof(DependencyInjection).Assembly)
//                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
//            .AsSelfWithInterfaces()
//            .WithScopedLifetime());
//    }
//}
