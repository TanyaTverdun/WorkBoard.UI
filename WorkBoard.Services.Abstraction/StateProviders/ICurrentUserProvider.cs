using WorkBoard.Services.Abstraction.DTOs.Users;

namespace WorkBoard.Services.Abstraction.StateProviders;

public interface ICurrentUserProvider
{
    Task<Guid?> GetUserIdAsync();
    Task<string?> GetEmailAsync();
    Task<string?> GetFullNameAsync();
    UserProfileDto? Profile { get; }
    Task LoadProfileAsync();
    event Action? OnProfileChanged;

    void NotifyProfileChanged();
}
