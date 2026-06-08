namespace WorkBoard.Services.Abstraction;

public interface IAuthService
{
    Task<Guid?> AuthenticateUserInBackendAsync(
        CancellationToken cancellationToken = default);
}
