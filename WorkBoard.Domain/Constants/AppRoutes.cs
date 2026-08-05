namespace WorkBoard.Domain.Constants;

public static class AppRoutes
{
    public const string Home = "/";
    public const string Login = "login";
    public const string Logout = "authentication/logout";
    public const string Profile = "profile";
    public const string NotFound = "not-found";
    public const string ArchivationTracker = "archivation";

    public static class Auth
    {
        public const string LoginAction = "authentication/login";
        public const string LogoutAction = "authentication/logout";
    }
}
