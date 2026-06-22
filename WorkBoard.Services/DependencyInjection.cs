using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using WorkBoard.Domain.Options;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Servises.Auth;
using WorkBoard.Services.Servises.Board;
using WorkBoard.Services.Servises.Section;
using WorkBoard.Services.Servises.Workspace;
using WorkBoard.Services.StateProviders;

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

        services.AddScoped<WorkspaceStateProvider>();
        services.AddSingleton<BoardStateService>();

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(backendBaseUrl))
            .AddHttpMessageHandler(CreateAuthorizationHandler);

        services.AddScoped<IAuthService, AuthService>();

        services.AddRefitClient<IWorkspaceApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(backendBaseUrl))
            .AddHttpMessageHandler(CreateAuthorizationHandler);

        services.AddScoped<IWorkspaceService, WorkspaceService>();

        services.AddRefitClient<IBoardApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(backendBaseUrl))
            .AddHttpMessageHandler(CreateAuthorizationHandler);

        services.AddScoped<IBoardService, BoardService>();

        services.AddRefitClient<ISectionApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(backendBaseUrl))
            .AddHttpMessageHandler(CreateAuthorizationHandler);

        services.AddScoped<ISectionService, SectionService>();

        return services;
    }
}
