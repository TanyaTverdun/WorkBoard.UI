using Microsoft.Extensions.DependencyInjection;

namespace WorkBoard.Services;

public static class DependencyInjection
{
    private const string ServiceSuffix = "Service";
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        var serviceTypes = typeof(DependencyInjection).Assembly.GetTypes()
                    .Where(t => 
                        t.IsClass && 
                        !t.IsAbstract && 
                        t.Name.EndsWith(ServiceSuffix));

        foreach (var implementationType in serviceTypes)
        {
            var interfaceType = implementationType.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implementationType.Name}");

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }
        return services;
    }
}
