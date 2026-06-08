using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using WorkBoard.Domain.Options;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Auth;

namespace WorkBoard.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string backendBaseUrl)
    {
        services.AddRefitClient<IAuthApi>()
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

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
