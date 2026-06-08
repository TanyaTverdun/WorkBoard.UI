namespace WorkBoard.Domain.Constants;

public static class ConfigConstants
{
    public const string HttpClientName = "WorkBoard.WebAPI";
    public const string AzureAdSectionName = "AzureAd";

    public static class AzureScopes
    {
        public const string AccessAsUser = "access_as_user";
    }

    public static class MsalLoginModes
    {
        public const string Redirect = "redirect";
        public const string Popup = "popup";
    }
}