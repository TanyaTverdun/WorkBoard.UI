namespace WorkBoard.Services.Abstraction.Services;

public interface IAuthService
{
    Task<Guid?> AuthenticateUserInBackendAsync(
        CancellationToken cancellationToken = default);
}
