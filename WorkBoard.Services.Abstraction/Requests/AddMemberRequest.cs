namespace WorkBoard.Services.Abstraction.Requests;

public record AddMemberRequest(
    Guid UserId,
    int Role);
