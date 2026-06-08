namespace WorkBoard.Services.Abstraction.Auth;

public interface IAuthService
{
    Task<Guid?> AuthenticateUserInBackendAsync(
        CancellationToken cancellationToken = default);
}
