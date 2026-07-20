namespace WorkBoard.Services.Abstraction.DTOs;

public record AttachmentDeletedDto(
    Guid CardId, 
    Guid AttachmentId);
