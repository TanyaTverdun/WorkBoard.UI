using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.Requests;

public record UpdateRoleRequest(
    BoardRole NewRole);
