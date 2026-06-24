using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs;

public record BoardMemberDto(
    Guid UserId,
    string FullName,
    string Initials,
    string Email,
    string? AvatarUrl,
    BoardRole UserRole);
