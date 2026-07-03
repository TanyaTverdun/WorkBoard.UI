namespace WorkBoard.Services.Abstraction.DTOs;

public class ChecklistDto
{
    public Guid ChecklistId { get; set; }
    public Guid CardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<ChecklistItemDto> Items { get; set; } =
        Array.Empty<ChecklistItemDto>();
}
