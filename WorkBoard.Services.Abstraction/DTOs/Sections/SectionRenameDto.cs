namespace WorkBoard.Services.Abstraction.DTOs.Sections;

public record SectionRenameDto(
    Guid SectionId, 
    string NewName);
