namespace WorkBoard.Services.Abstraction.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }

    public Guid CardId { get; set; }

    public Guid UserId { get; set; }

    public required string Text { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? UserFullName { get; set; }

    public required string Initials { get; set; }
}
