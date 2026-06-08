namespace WorkBoard.Domain.Constants;

public static class UiConstants
{
    public static class Auth
    {
        public const string DefaultUsername = "User";
        public const string LoadingText = "Loading...";
    }

    public static class Marketing
    {
        public static readonly string[] LoginFeatures = new[]
        {
            "Role-based access control",
            "AI-powered task assistant",
            "WebRTC live meetings",
            "Board archivation & restore"
        };
    }
}