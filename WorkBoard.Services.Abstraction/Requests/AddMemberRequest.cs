using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.Requests;

public record AddMemberRequest(
    Guid UserId,
    BoardRole Role);
