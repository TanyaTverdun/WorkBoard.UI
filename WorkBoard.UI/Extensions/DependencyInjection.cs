using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WorkBoard.UI.Constants;
using WorkBoard.UI.Options;
using WorkBoard.UI.Services;

namespace WorkBoard.UI.Extensions;

public static class DependencyInjection
{
    public static WebAssemblyHostBuilder AddUiServices(
        this WebAssemblyHostBuilder builder)
    {
        var uiOptions = builder.Configuration.Get<WorkBoardUiOptions>()
            ?? throw new InvalidOperationException("Root configuration is missing.");

        var azureOptions = builder.Configuration
            .GetSection(ConfigConstants.AzureAdSectionName)
            .Get<AzureAdOptions>()
            ?? throw new InvalidOperationException(
                $"{ConfigConstants.AzureAdSectionName} section is missing.");

        builder.Services.AddHttpClient(ConfigConstants.HttpClientName, client =>
            client.BaseAddress = new Uri(uiOptions.BackendBaseUrl))
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
            .CreateClient(ConfigConstants.HttpClientName));

        builder.Services.AddScoped<AuthService>();

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
