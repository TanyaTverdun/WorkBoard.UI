using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs;

public record MemberRoleUpdatedDto(
    Guid UserId, 
    BoardRole NewRole);
