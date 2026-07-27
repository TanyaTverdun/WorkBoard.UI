namespace WorkBoard.Services.Abstraction.DTOs.Users;

public class UserSearchDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? AvatarColor { get; set; }
    public string? Initials { get; set; }
}
