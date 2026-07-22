namespace WorkBoard.Services.Abstraction.DTOs.Sections;
    public record SectionMoveDto(
        Guid SectionId, 
        double NewPosition);
