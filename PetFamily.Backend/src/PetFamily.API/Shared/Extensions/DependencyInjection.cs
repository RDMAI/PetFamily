using Microsoft.AspNetCore.Mvc;

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

        // disables controller's filter, that validaties model state before entering controller;
        // instead, it will pass invalid model to controller and then tp application layer,
        // where we validate the model with fluent validation
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }
}
