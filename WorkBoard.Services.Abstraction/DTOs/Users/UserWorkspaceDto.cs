using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs.Users;

public record UserWorkspaceDto(
    Guid Id,
    string Name,
    SubscriptionTier SubscriptionTier,
    WorkspaceRole UserRole
);
