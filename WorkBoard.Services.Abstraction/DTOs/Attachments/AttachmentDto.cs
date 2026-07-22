namespace WorkBoard.Services.Abstraction.DTOs.Attachments;

public class AttachmentDto
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
}
