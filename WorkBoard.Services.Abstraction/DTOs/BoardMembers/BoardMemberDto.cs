using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs.BoardMembers;

public record BoardMemberDto(
    Guid UserId,
    string FullName,
    string Initials,
    string Email,
    string? AvatarUrl,
    string? AvatarColor,
    BoardRole UserRole);
