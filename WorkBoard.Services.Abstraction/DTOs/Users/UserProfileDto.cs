namespace WorkBoard.Services.Abstraction.DTOs.Users;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? AvatarUrl { get; set; }
    public string? AvatarColor { get; set; }
    public required string Initials { get; set; }
}
