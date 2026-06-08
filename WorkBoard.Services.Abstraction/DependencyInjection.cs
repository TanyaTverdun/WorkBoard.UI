using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using WorkBoard.Domain.Options;

namespace WorkBoard.Services.Abstraction;

public static class DependencyInjection
{
    private const string ApiSuffix = "Api";

    public static IServiceCollection AddInfrastructureAbstractions(
        this IServiceCollection services,
        string backendBaseUrl)
    {
        var apiInterfaces = typeof(DependencyInjection).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith(ApiSuffix));

        foreach (var apiInterface in apiInterfaces)
        {
            services.AddRefitClient(apiInterface)
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(backendBaseUrl))

                .AddHttpMessageHandler(sp =>
                {
                    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();

                    var azureOptions = sp.GetRequiredService<IOptions<AzureAdOptions>>().Value;

                    var backendScope = $"api://{azureOptions.BackendClientId}/access_as_user";

                    handler.ConfigureHandler(
                        authorizedUrls: new[] { backendBaseUrl },
                        scopes: new[] { backendScope });

                    return handler;
                });
        }

        return services;
    }
}
