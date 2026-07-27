using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs.Users;

public record MemberRoleUpdatedDto(
    Guid UserId, 
    BoardRole NewRole);
