namespace WorkBoard.Services.Abstraction.DTOs;

public class LabelDto
{
    public Guid Id { get; set; }

    public Guid BoardId { get; set; }

    public required string Name { get; set; }

    public string? Color { get; set; }
}
