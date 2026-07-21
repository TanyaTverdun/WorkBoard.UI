namespace WorkBoard.Services.Abstraction.StateProviders;

public interface ICurrentUserProvider
{
    Task<Guid?> GetUserIdAsync();
    Task<string?> GetEmailAsync();
    Task<string?> GetFullNameAsync();
}
