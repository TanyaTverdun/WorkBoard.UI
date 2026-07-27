namespace WorkBoard.Services.Abstraction.Requests.Users;

public record UpdateUserAvatarColorRequest
{
    public required string AvatarColor { get; init; }
}
