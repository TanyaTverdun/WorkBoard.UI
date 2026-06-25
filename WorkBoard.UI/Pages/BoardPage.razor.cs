using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using MudBlazor;
using WorkBoard.Domain.Enums;
using WorkBoard.Domain.Options;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.StateProviders;
using WorkBoard.UI.ViewModels.Board;

namespace WorkBoard.UI.Pages;

public partial class BoardPage
{
    [Inject]
    private ISectionService SectionService { get; set; } = default!;

    [Inject]
    private IBoardService BoardService { get; set; } = default!;

    [Inject]
    private IBoardMembersService BoardMembersService { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    [Inject]
    private BoardStateService BoardStateService { get; set; } = default!;

    [Inject]
    private WorkspaceStateProvider WorkspaceStateProvider { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ICardService CardService { get; set; } = default!;

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private IOptions<WorkBoardUiOptions> UiOptions { get; set; } = default!;

    [Parameter]
    public Guid BoardIdGuid { get; set; }

    private Guid? WorkspaceId => WorkspaceStateProvider.SelectedWorkspaceId;

    private MudDropContainer<KanbanTaskViewModel> _dropContainer = default!;
    private bool _addSectionOpen;

    private List<KanbanSectionViewModel> _sections = new();
    private List<KanbanTaskViewModel> _tasks = new();
    private List<BoardMemberViewModel>? _boardMembers = new();

    private CreateSectionForm newSectionModel = new CreateSectionForm();

    private bool _isReorderPopoverOpen;
    private List<KanbanSectionViewModel> _reorderList = new();

    private bool _isMembersPopoverOpen;
    private BoardRole _newMemberRole = BoardRole.Member;

    private Guid? _currentUserId;
    private UserSearchDto? _selectedUserToAdd;
    private bool IsCurrentUserObserver =>
        _boardMembers?.FirstOrDefault(m => m.Dto.UserId == _currentUserId)?.Dto.UserRole == BoardRole.Observer;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();

        var userIdString = authState.User.FindFirst(c =>
            c.Type == "oid" ||
            c.Type == "sub" ||
            c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdString, out var parsedId))
        {
            _currentUserId = parsedId;
        }

        BoardStateService.OnBoardNameChanged += StateHasChanged;

        BoardHubService.OnCardCreated += HandleCardCreated;
        BoardHubService.OnSectionCreated += HandleSectionCreated;
        BoardHubService.OnSectionRenamed += HandleSectionRenamed;
        BoardHubService.OnSectionDeleted += HandleSectionDeleted;

        try
        {
            var backendUrl = UiOptions.Value.BackendBaseUrl;

            Console.WriteLine($"URL {backendUrl}");

            await BoardHubService.StartConnectionAsync(backendUrl, BoardIdGuid);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignalR Error: {ex.Message}");
            Snackbar.Add("Working in offline mode. Live updates are unavailable", Severity.Warning);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (WorkspaceId == null)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        var board = await BoardService.GetBoardAsync(
            WorkspaceId.Value,
            BoardIdGuid);

        BoardStateService.SetBoardName(board.Name);

        var dtos = await BoardMembersService.GetBoardMembersAsync(
            WorkspaceId.Value,
            BoardIdGuid);
        _boardMembers = dtos
            .Select(dto => new BoardMemberViewModel(dto))
            .ToList();

        var sectionsFromDb = await SectionService
            .GetSectionsByBoardAsync(BoardIdGuid);

        _sections = sectionsFromDb
            .OrderBy(s => s.Position)
            .Select(s => new KanbanSectionViewModel(
                s.Id,
                s.Name,
                false,
                string.Empty)
            {
                Position = s.Position
            }).ToList();

        var cardsFromDb = await CardService.GetCardsByBoardAsync(BoardIdGuid);

        _tasks = cardsFromDb.Select(c =>
        {
            var sectionName = _sections.FirstOrDefault(s => s.Id == c.SectionId)?.Name ?? string.Empty;

            return new KanbanTaskViewModel(c.Id, c.Title, sectionName);

        }).ToList();
    }

    private void TaskUpdated(MudItemDropInfo<KanbanTaskViewModel> info)
    {
        if (info.Item is null)
        {
            return;
        }

        info.Item.Status = info.DropzoneIdentifier;
    }

    private async Task OnValidSectionSubmit(EditContext context)
    {
        var request = new CreateSectionRequest
        {
            Name = newSectionModel.Name
        };

        try
        {
            await SectionService.CreateSectionAsync(BoardIdGuid, request);

            newSectionModel.Name = string.Empty;
            _addSectionOpen = false;

        }
        catch (Exception)
        {
            Snackbar.Add("Failed to create sectionю", Severity.Error);
        }
    }

    private async Task SaveRename(KanbanSectionViewModel section)
    {
        if (string.IsNullOrWhiteSpace(section.EditName))
        {
            section.IsRenaming = false;
            return;
        }

        var newName = section.EditName.Trim();

        if (newName == section.Name)
        {
            section.IsRenaming = false;
            return;
        }

        var request = new UpdateSectionNameRequest
        {
            Name = newName
        };

        try
        {
            await SectionService.RenameSectionAsync(
                BoardIdGuid,
                section.Id,
                request);

            section.IsRenaming = false;
        }
        catch (Exception)
        {
            Snackbar.Add("Failed to rename section", Severity.Error);
        }
    }

    private async Task DeleteSection(KanbanSectionViewModel section)
    {
        try
        {
            await SectionService.DeleteSectionAsync(
                BoardIdGuid, 
                section.Id);
        }
        catch (Exception)
        {
            Snackbar.Add("Failed to delete section", Severity.Error);
        }
    }

    private void StartRename(KanbanSectionViewModel section)
    {
        section.EditName = section.Name;
        section.IsRenaming = true;
        section.MenuOpen = false;
    }

    private void OpenAddNewSection()
    {
        _addSectionOpen = true;
    }

    private void CloseAddNewSection()
    {
        _addSectionOpen = false;
        newSectionModel.Name = string.Empty;
    }

    private async Task AddTask(KanbanSectionViewModel section)
    {
        if (string.IsNullOrWhiteSpace(section.NewTaskName))
        {
            return;
        }

        var currentCardsInSection = _tasks.Where(t => t.Status == section.Name).ToList();
        double nextPosition = currentCardsInSection.Count > 0
            ? currentCardsInSection.Count + 1.0
            : 1.0;

        var request = new CreateCardRequest(
            section.NewTaskName,
            nextPosition
        );

        try
        {
            await CardService.CreateCardAsync(section.Id, request);

            section.NewTaskName = string.Empty;
            section.NewTaskOpen = false;
        }
        catch (Exception)
        {
            Snackbar.Add("Failed to create task", Severity.Error);
        }
    }

    private void CloseNewTaskForm(KanbanSectionViewModel section)
    {
        section.NewTaskOpen = false;
        section.NewTaskName = string.Empty;
    }

    private void OpenReorderPopover()
    {
        _reorderList = _sections.ToList();
        _isReorderPopoverOpen = true;
    }

    private void CloseReorderPopover()
    {
        _isReorderPopoverOpen = false;
    }

    private void SectionDropped(
        MudItemDropInfo<KanbanSectionViewModel> info)
    {
        var item = info.Item;

        if (item is null)
        {
            return;
        }

        _reorderList.Remove(item);
        _reorderList.Insert(info.IndexInZone, item);

        double prevPos = info.IndexInZone > 0 ?
            _reorderList[info.IndexInZone - 1].Position : 0.0;

        double nextPos = info.IndexInZone < _reorderList.Count - 1
            ? _reorderList[info.IndexInZone + 1].Position
            : prevPos + 1.0;

        item.Position = prevPos == 0.0 ?
            nextPos / 2.0 : (prevPos + nextPos) / 2.0;

        item.IsPositionChanged = true;
    }

    private async Task ApplySectionOrderAsync()
    {
        var movedSections = _reorderList
            .Where(s => s.IsPositionChanged)
            .ToList();

        foreach (var section in movedSections)
        {
            var request = new MoveSectionRequest
            {
                NewPosition = section.Position
            };

            await SectionService.MoveSectionAsync(
                BoardIdGuid,
                section.Id,
                request);

            section.IsPositionChanged = false;
        }

        _sections = _reorderList
            .OrderBy(s => s.Position)
            .ToList();

        _isReorderPopoverOpen = false;
        _dropContainer.Refresh();
    }

    private void OpenManageMembersDialog()
    {
        _isMembersPopoverOpen = true;
    }

    private void CloseManageMembersDialog()
    {
        _isMembersPopoverOpen = false;
        _newMemberRole = BoardRole.Member;
    }

    private async Task UpdateRoleAsync(
        BoardMemberViewModel member, 
        BoardRole newRole)
    {
        if (member.Dto.UserRole == newRole)
        {
            return;
        }

        var request = new UpdateRoleRequest((int)newRole);

        await BoardMembersService.UpdateMemberRoleAsync(
            WorkspaceId.Value,
            BoardIdGuid,
            member.Dto.UserId,
            request);

        var updatedMembers = _boardMembers!.ToList();
        var index = updatedMembers.FindIndex(m => m.Dto.UserId == member.Dto.UserId);

        if (index != -1)
        {
            updatedMembers[index] = new BoardMemberViewModel(member.Dto with
            {
                UserRole = newRole
            });
            _boardMembers = updatedMembers;
        }
    }

    private async Task RemoveMemberAsync(BoardMemberViewModel member)
    {
        await BoardMembersService.RemoveBoardMemberAsync(
            WorkspaceId.Value,
            BoardIdGuid,
            member.Dto.UserId);

        _boardMembers = _boardMembers!
            .Where(m => m.Dto.UserId != member.Dto.UserId)
            .ToList();
    }

    private async Task AddMemberAsync()
    {
        if (_selectedUserToAdd == null || WorkspaceId == null)
        {
            return;
        }

        try
        {
            var request = new AddMemberRequest(
                _selectedUserToAdd.UserId,
                (int)_newMemberRole);

            await BoardMembersService.AddBoardMemberAsync(
                WorkspaceId.Value,
                BoardIdGuid,
                request);

            var dtos = await BoardMembersService.GetBoardMembersAsync(
                WorkspaceId.Value,
                BoardIdGuid);
            _boardMembers = dtos
                .Select(dto => new BoardMemberViewModel(dto))
                .ToList();

            _selectedUserToAdd = null;
            _newMemberRole = BoardRole.Member;

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding member: {ex.Message}");
        }
    }

    private async Task<IEnumerable<UserSearchDto>> SearchUsersAsync(
        string value,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Length < 2 ||
            WorkspaceId == null)
        {
            return new List<UserSearchDto>();
        }

        try
        {
            var result = await UserService.SearchAssignableUsersAsync(
                BoardIdGuid,
                value);

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Search failed: {ex.Message}");
            return new List<UserSearchDto>();
        }
    }

    private void HandleCardCreated(CardDto newCard)
    {
        if (_tasks.Any(t => t.Id == newCard.Id))
        {
            return;
        }

        var targetSection = _sections.FirstOrDefault(
            s => s.Id == newCard.SectionId);

        if (targetSection != null)
        {
            var newTask = new KanbanTaskViewModel(
                newCard.Id, 
                newCard.Title, 
                targetSection.Name);

            _tasks.Add(newTask);

            InvokeAsync(() =>
            {
                StateHasChanged();
                _dropContainer.Refresh();
            });
        }
    }

    private void HandleSectionCreated(SectionDto newSection)
    {
        if (_sections.Any(s => s.Id == newSection.Id))
        {
            return;
        }

        _sections.Add(new KanbanSectionViewModel(
            newSection.Id,
            newSection.Name,
            false,
            string.Empty)
        {
            Position = newSection.Position
        });

        InvokeAsync(() =>
        {
            StateHasChanged();
            _dropContainer.Refresh();
        });
    }

    private void HandleSectionRenamed(SectionRenameDto data)
    {
        var section = _sections.FirstOrDefault(s => s.Id == data.SectionId);

        if (section != null && section.Name != data.NewName)
        {
            string oldName = section.Name;
            section.Name = data.NewName;

            var tasksToUpdate = _tasks.Where(t => t.Status == oldName).ToList();
            foreach (var t in tasksToUpdate)
            {
                t.Status = data.NewName;
            }

            InvokeAsync(() =>
            {
                StateHasChanged();
                _dropContainer.Refresh();
            });
        }
    }

    private void HandleSectionDeleted(Guid sectionId)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);

        if (section != null)
        {
            _sections.Remove(section);
            _tasks.RemoveAll(t => t.Status == section.Name);

            InvokeAsync(() =>
            {
                StateHasChanged();
                _dropContainer.Refresh();
            });
        }
    }

    public async ValueTask DisposeAsync()
    {
        BoardStateService.OnBoardNameChanged -= StateHasChanged;
        BoardHubService.OnCardCreated -= HandleCardCreated;
        BoardHubService.OnSectionCreated -= HandleSectionCreated;
        BoardHubService.OnSectionRenamed -= HandleSectionRenamed;
        BoardHubService.OnSectionDeleted -= HandleSectionDeleted;

        await BoardHubService.StopConnectionAsync(BoardIdGuid);
    }

}