using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.Abstraction.StateProviders;

namespace WorkBoard.UI.Pages;

public partial class Profile : IDisposable
{
    [Inject]
    private ICurrentUserProvider CurrentUserProvider { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    private readonly string[] AvatarColors =
    {
        "#3b82f6",
        "#8b5cf6",
        "#10b981",
        "#f59e0b",
        "#ef4444",
        "#ec4899",
        "#84cc16",
        "#475569",
    };

    private string? _originalColor = "#64748b";
    public string? SelectedColor { get; set; }
    private bool _isPickerOpen;
    private bool _isCustomColorSelected;

    private bool HasUnsavedChanges => SelectedColor != _originalColor;

    protected override void OnInitialized()
    {
        BoardHubService.OnUserAvatarUpdated += HandleUserAvatarUpdated;
        CurrentUserProvider.OnProfileChanged += HandleProfileChanged;

        InitializeColors();
    }

    private void InitializeColors()
    {
        if (CurrentUserProvider.Profile != null)
        {
            _originalColor = CurrentUserProvider.Profile.AvatarColor ?? "#64748b";
            SelectedColor = _originalColor;
        }
    }

    private void CancelChanges()
    {
        SelectedColor = _originalColor;
    }

    private async Task SaveChangesAsync()
    {
        try
        {
            await UserService.UpdateAvatarColorAsync(SelectedColor);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to update profile color.", Severity.Error);
            Console.WriteLine(ex);
        }
    }

    private async Task UploadAvatarAsync(InputFileChangeEventArgs e)
    {
        var file = e.File;

        if (file == null)
        {
            return;
        }

        var maxAllowedSize = 5 * 1024 * 1024;
        if (file.Size > maxAllowedSize)
        {
            Snackbar.Add(
                "File is too large. Maximum size is 5 MB.", 
                Severity.Error);

            return;
        }

        try
        {
            using var stream = file.OpenReadStream(maxAllowedSize);

            await UserService.UploadAvatarImageAsync(
                stream,
                file.Name,
                file.ContentType);
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                "Failed to upload profile photo.", 
                Severity.Error);

            Console.WriteLine(ex);
        }
    }

    private void SelectColor(string color)
    {
        SelectedColor = color;
        _isCustomColorSelected = false; 
        _isPickerOpen = false;          
    }

    private void ToggleColorPicker()
    {
        _isCustomColorSelected = true; 
        _isPickerOpen = !_isPickerOpen;
    }

    private void HandleUserAvatarUpdated(UserAvatarUpdatedDto data)
    {
        if (CurrentUserProvider.Profile != null && 
            CurrentUserProvider.Profile.Id == data.UserId)
        {
            CurrentUserProvider.Profile.AvatarColor = data.AvatarColor;
            _originalColor = data.AvatarColor;
            SelectedColor = data.AvatarColor;
            CurrentUserProvider.Profile.AvatarUrl = data.AvatarUrl;

            CurrentUserProvider.NotifyProfileChanged();
            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleProfileChanged()
    {
        InitializeColors();
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        BoardHubService.OnUserAvatarUpdated -= HandleUserAvatarUpdated;
        CurrentUserProvider.OnProfileChanged -= HandleProfileChanged;

    }
}
