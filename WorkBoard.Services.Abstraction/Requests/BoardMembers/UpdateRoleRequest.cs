using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.Requests.BoardMembers;

public record UpdateRoleRequest(
    int NewRole);
