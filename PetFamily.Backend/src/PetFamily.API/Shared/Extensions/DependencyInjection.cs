namespace PetFamily.API.Shared.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services)
    {
        //services.AddFluentValidationAutoValidation(configuration =>
        //{
        //    configuration.EnableCustomBindingSourceAutomaticValidation = true;

        //    // Replace the default result factory with a custom implementation.
        //    configuration.OverrideDefaultResultFactoryWith<CustomValidationResultFactory>();
        //});

        return services;
    }
}
