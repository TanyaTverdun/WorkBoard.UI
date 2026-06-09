using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using WorkBoard.Domain.Options;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Auth;
using WorkBoard.Services.Workspace;

namespace WorkBoard.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string backendBaseUrl)
    {
        AuthorizationMessageHandler CreateAuthorizationHandler(IServiceProvider sp)
        {
            var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
            var azureOptions = sp.GetRequiredService<IOptions<AzureAdOptions>>().Value;
            var backendScope = $"api://{azureOptions.BackendClientId}/access_as_user";

            handler.ConfigureHandler(
                authorizedUrls: new[] { backendBaseUrl },
                scopes: new[] { backendScope });

            return handler;
        }

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(backendBaseUrl))
            .AddHttpMessageHandler(CreateAuthorizationHandler);

        services.AddScoped<IAuthService, AuthService>();

        services.AddRefitClient<IWorkspaceApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(backendBaseUrl))
            .AddHttpMessageHandler(CreateAuthorizationHandler);

        services.AddScoped<IWorkspaceService, WorkspaceService>();

        return services;
    }
}
