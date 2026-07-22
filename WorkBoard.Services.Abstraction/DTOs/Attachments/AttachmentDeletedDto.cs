namespace WorkBoard.Services.Abstraction.DTOs.Attachments;

public record AttachmentDeletedDto(
    Guid CardId, 
    Guid AttachmentId);
