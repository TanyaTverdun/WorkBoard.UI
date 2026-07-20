namespace WorkBoard.Services.Abstraction.DTOs;

public record AttachmentAddedDto(
    Guid CardId, 
    AttachmentDto Attachment);
