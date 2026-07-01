namespace WorkBoard.Services.Abstraction.DTOs;

public class CreateLabelRequest
{
    public required string Name { get; set; }
    public string? Color { get; set; }
}
