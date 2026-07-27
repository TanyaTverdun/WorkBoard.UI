namespace WorkBoard.Services.Abstraction.Requests.Labels;

public class CreateLabelRequest
{
    public required string Name { get; set; }
    public string? Color { get; set; }
}
