namespace WorkBoard.Services.Abstraction.DTOs.Users;

public record UserAvatarUpdatedDto
{
    public Guid UserId { get; init; }
    public string? AvatarColor { get; init; }
    public string? AvatarUrl { get; set; }
}
