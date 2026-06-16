namespace WorkBoard.Services.Abstraction.DTOs;

public class SectionDto
{
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Position { get; set; }
}
