using Refit;

namespace WorkBoard.Services.Servises.Auth;

internal interface IAuthApi
{
    [Post("/api/users/auth")]
    Task<Guid?> AuthenticateUserInBackendAsync(
        CancellationToken cancellationToken = default);
}
