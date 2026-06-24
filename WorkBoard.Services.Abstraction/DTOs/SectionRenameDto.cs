namespace WorkBoard.Services.Abstraction.DTOs;

public record SectionRenameDto(
    Guid SectionId, 
    string NewName);
