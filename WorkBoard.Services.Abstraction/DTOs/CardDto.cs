namespace WorkBoard.Services.Abstraction.DTOs;

public class CardDto
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public double Position { get; set; }
}
