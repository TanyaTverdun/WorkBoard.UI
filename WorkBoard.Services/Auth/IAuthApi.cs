using Refit;

namespace WorkBoard.Services.Auth;

internal interface IAuthApi
{
    [Post("/api/users/auth")]
    Task<Guid?> AuthenticateUserInBackendAsync(
        CancellationToken cancellationToken = default);
}
