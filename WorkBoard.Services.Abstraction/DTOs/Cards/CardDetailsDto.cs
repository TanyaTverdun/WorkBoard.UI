using WorkBoard.Services.Abstraction.DTOs.ActivityLogs;
using WorkBoard.Services.Abstraction.DTOs.Attachments;
using WorkBoard.Services.Abstraction.DTOs.Checklists;
using WorkBoard.Services.Abstraction.DTOs.Comments;
using WorkBoard.Services.Abstraction.DTOs.Labels;

namespace WorkBoard.Services.Abstraction.DTOs.Cards;

public class CardDetailsDto
{
    public Guid Id { get; set; }

    public Guid SectionId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public double Position { get; set; }

    public IReadOnlyList<CardAssigneeDto> Assignees { get; set; } =
        new List<CardAssigneeDto>();

    public IReadOnlyList<LabelDto> Labels { get; set; } =
        new List<LabelDto>();

    public ChecklistDto? Checklist { get; set; }

    public IReadOnlyList<AttachmentDto> Attachments { get; set; } =
        new List<AttachmentDto>();

    public IReadOnlyList<CommentDto> Comments { get; set; } =
        new List<CommentDto>();

    public IReadOnlyList<ActivityLogDto> ActivityLogs { get; set; } =
        new List<ActivityLogDto>();
}
