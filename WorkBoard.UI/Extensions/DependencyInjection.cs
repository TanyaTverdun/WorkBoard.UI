using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WorkBoard.Domain.Constants;
using WorkBoard.Domain.Options;
using WorkBoard.Services;

namespace WorkBoard.UI.Extensions;

public static class DependencyInjection
{
    public static WebAssemblyHostBuilder AddUiServices(
        this WebAssemblyHostBuilder builder)
    {
        var uiOptions = builder.Configuration.Get<WorkBoardUiOptions>()
            ?? throw new InvalidOperationException(
                "Root configuration is missing.");

        var azureAdSection = builder.Configuration.GetSection(
            ConfigConstants.AzureAdSectionName);

        var azureOptions = azureAdSection.Get<AzureAdOptions>()
            ?? throw new InvalidOperationException(
                $"{ConfigConstants.AzureAdSectionName} section is missing.");

        builder.Services.Configure<AzureAdOptions>(azureAdSection);

        builder.Services
            .AddInfrastructureServices(uiOptions.BackendBaseUrl);

        builder.Services.AddMsalAuthentication(options =>
        {
            options.ProviderOptions.Authentication.Authority = 
                azureOptions.Authority;
            options.ProviderOptions.Authentication.ClientId = 
                azureOptions.ClientId;
            options.ProviderOptions.Authentication.ValidateAuthority = 
                azureOptions.ValidateAuthority;
            options.ProviderOptions.LoginMode = 
                ConfigConstants.MsalLoginModes.Redirect;

            options.ProviderOptions.DefaultAccessTokenScopes.Add(
                $"api://{azureOptions.BackendClientId}/" +
                $"{ConfigConstants.AzureScopes.AccessAsUser}");
        });

        return builder;
    }
}
