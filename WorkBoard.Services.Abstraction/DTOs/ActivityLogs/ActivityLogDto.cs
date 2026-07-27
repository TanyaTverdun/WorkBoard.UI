namespace WorkBoard.Services.Abstraction.DTOs.ActivityLogs;

public class ActivityLogDto
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? AvatarColor { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
