using Refit;
using WorkBoard.Services.Abstraction.Constants;

namespace WorkBoard.Services.Abstraction.Auth;

public interface IAuthApi
{
    [Post("/" + ApiEndpoints.Auth.Authenticate)]
    Task<Guid?> AuthenticateUserInBackendAsync(
        CancellationToken cancellationToken = default);
}
