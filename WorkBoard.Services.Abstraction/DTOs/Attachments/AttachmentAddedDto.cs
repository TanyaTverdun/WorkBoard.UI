namespace WorkBoard.Services.Abstraction.DTOs.Attachments;

public record AttachmentAddedDto(
    Guid CardId, 
    AttachmentDto Attachment);
