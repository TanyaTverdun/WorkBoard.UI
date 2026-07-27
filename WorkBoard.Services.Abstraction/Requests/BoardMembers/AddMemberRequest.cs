namespace WorkBoard.Services.Abstraction.Requests.BoardMembers;

public record AddMemberRequest(
    Guid UserId,
    int Role);
