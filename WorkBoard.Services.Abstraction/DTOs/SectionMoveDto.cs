namespace WorkBoard.Services.Abstraction.DTOs;
    public record SectionMoveDto(
        Guid SectionId, 
        double NewPosition);
