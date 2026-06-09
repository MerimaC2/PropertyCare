using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PropertyCare.Application.Common.Behaviors;

namespace PropertyCare.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR handlers from the Application layer
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation validators from the Application layer
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline behaviors (validation runs before every handler)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
