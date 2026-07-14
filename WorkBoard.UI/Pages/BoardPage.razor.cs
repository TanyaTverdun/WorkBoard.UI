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
using WorkBoard.Services.Abstraction.Requestsж;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.StateProviders;
using WorkBoard.UI.Components.Boards;
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

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

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

    private bool IsWorkspaceObserver =>
        WorkspaceStateProvider.CurrentRole == WorkspaceRole.Observer;
    private bool IsCurrentUserObserver =>
        _boardMembers?.FirstOrDefault(
            m => m.Dto.UserId == _currentUserId)?.Dto.UserRole == BoardRole.Observer;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();

        var userIdString = authState.User.FindFirst(c => c.Type == "oid")?.Value;

        if (string.IsNullOrEmpty(userIdString))
        {
            userIdString = authState.User.FindFirst(c => 
                c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        if (Guid.TryParse(userIdString, out var parsedId))
        {
            _currentUserId = parsedId;
        }

        BoardStateService.OnBoardNameChanged += StateHasChanged;

        BoardHubService.OnCardCreated += HandleCardCreated;
        BoardHubService.OnSectionCreated += HandleSectionCreated;
        BoardHubService.OnSectionRenamed += HandleSectionRenamed;
        BoardHubService.OnSectionDeleted += HandleSectionDeleted;
        BoardHubService.OnSectionMoved += HandleSectionMoved;
        BoardHubService.OnMemberRoleUpdated += HandleMemberRoleUpdated;
        BoardHubService.OnMemberRemoved += HandleMemberRemoved;
        BoardHubService.OnCardMoved += HandleCardMoved;
        BoardHubService.OnCardDeleted += HandleCardDeleted;
        BoardHubService.OnCardRenamed += HandleCardRenamed;
        BoardHubService.OnCardDueDateUpdated += HandleCardDueDateUpdated;

        try
        {
            var backendUrl = UiOptions.Value.BackendBaseUrl;

            await BoardHubService.StartConnectionAsync(backendUrl, BoardIdGuid);
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                "Working in offline mode. Live updates are unavailable", 
                Severity.Warning);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (WorkspaceId == null)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        try
        {
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
                var sectionName = _sections.FirstOrDefault(
                    s => s.Id == c.SectionId)?.Name ?? string.Empty;

                return new KanbanTaskViewModel(
                    c.Id,
                    c.Title,
                    sectionName,
                    c.Position,
                    BoardIdGuid,
                    c.SectionId,
                    c.Description,
                    c.DueDate);

            }).OrderBy(t => t.Position).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                "You don't have access to this board anymore.", 
                Severity.Warning);

            NavigationManager.NavigateTo("/");
        }
    }

    private async Task TaskUpdated(MudItemDropInfo<KanbanTaskViewModel> info)
    {
        if (info.Item is null)
        {
            return;
        }

        var oldSectionName = info.Item.Status;
        info.Item.Status = info.DropzoneIdentifier;

        var targetSection = _sections.FirstOrDefault(
            s => s.Name == info.DropzoneIdentifier);

        if (targetSection == null)
        {
            return;
        }

        var cardsInSection = _tasks
            .Where(t => t.Status == targetSection.Name && t.Id != info.Item.Id)
            .OrderBy(t => t.Position)
            .ToList();

        double newPosition;

        if (cardsInSection.Count == 0)
        {
            newPosition = 1.0;
        }
        else if (info.IndexInZone <= 0)
        {
            newPosition = cardsInSection.First().Position / 2.0;
        }
        else if (info.IndexInZone >= cardsInSection.Count)
        {
            newPosition = cardsInSection.Last().Position + 1.0;
        }
        else
        {
            double prevPos = cardsInSection[info.IndexInZone - 1].Position;
            double nextPos = cardsInSection[info.IndexInZone].Position;
            newPosition = (prevPos + nextPos) / 2.0;
        }

        info.Item.Position = newPosition;
        _tasks = _tasks.OrderBy(t => t.Position).ToList();

        var request = new MoveCardRequest(targetSection.Id, newPosition);

        try
        {
            await CardService.MoveCardAsync(BoardIdGuid, info.Item.Id, request);
        }
        catch (Exception)
        {
            info.Item.Status = oldSectionName;
            Snackbar.Add("Failed to move card", Severity.Error);

            StateHasChanged();
            _dropContainer.Refresh();
        }
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

            try
            {
                await SectionService.MoveSectionAsync(
                    BoardIdGuid, 
                    section.Id, 
                    request);

                section.IsPositionChanged = false;
            }
            catch (Exception)
            {
                Snackbar.Add("Failed to save section order", Severity.Error);
            }
        }

        _isReorderPopoverOpen = false;
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

        var updatedMembers = _boardMembers!.ToList();
        var index = updatedMembers.FindIndex(
            m => m.Dto.UserId == member.Dto.UserId);

        if (index != -1)
        {
            updatedMembers[index] = new BoardMemberViewModel(member.Dto with
            {
                UserRole = newRole
            });

            _boardMembers = updatedMembers;
        }

        var request = new UpdateRoleRequest((int)newRole);

        try
        {
            if (WorkspaceId != null)
            {
                await BoardMembersService.UpdateMemberRoleAsync(
                    WorkspaceId.Value,
                    BoardIdGuid,
                    member.Dto.UserId,
                    request);
            }
        }
        catch (Exception)
        {
            Snackbar.Add("Failed to update role", Severity.Error);
        }
    }

    private async Task RemoveMemberAsync(BoardMemberViewModel member)
    {
        try
        {
            await BoardMembersService.RemoveBoardMemberAsync(
                WorkspaceId.Value,
                BoardIdGuid,
                member.Dto.UserId);

        }
        catch (Exception)
        {
            Snackbar.Add("Failed to remove member", Severity.Error);
        }
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

    private async Task OpenCardDetails(KanbanTaskViewModel card)
    {
        var parameters = new DialogParameters<CardDetails>
    {
        { x => x.Card, card },
        { x => x.CurrentUserId, _currentUserId ?? Guid.Empty }
    };

        var options = new DialogOptions
        {
            NoHeader = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            BackdropClick = true
        };

        await DialogService.ShowAsync<CardDetails>(string.Empty, parameters, options);
    }

    private void HandleCardCreated(CardDto newCard)
    {
        if (_tasks.Any(t => t.Id == newCard.Id)) return;

        var targetSection = _sections.FirstOrDefault(s => s.Id == newCard.SectionId);

        if (targetSection != null)
        {
            var newTask = new KanbanTaskViewModel(
                newCard.Id,
                newCard.Title,
                targetSection.Name,
                newCard.Position,
                BoardIdGuid,
                targetSection.Id,
                newCard.Description,
                null);

            _tasks.Add(newTask);

            _tasks = _tasks.OrderBy(t => t.Position).ToList();

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

    private void HandleSectionMoved(Guid sectionId, double newPosition)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);

        if (section != null)
        {
            section.Position = newPosition;

            _sections = _sections.OrderBy(s => s.Position).ToList();

            InvokeAsync(() =>
            {
                StateHasChanged();
                _dropContainer.Refresh();
            });
        }
    }

    private void HandleMemberRoleUpdated(Guid userId, BoardRole newRole)
    {
        if (_boardMembers == null) return;

        var index = _boardMembers.FindIndex(m => m.Dto.UserId == userId);

        if (index != -1 && _boardMembers[index].Dto.UserRole != newRole)
        {
            var updatedMembers = _boardMembers.ToList();
            updatedMembers[index] = new BoardMemberViewModel(
                _boardMembers[index].Dto with
                {
                    UserRole = newRole
                });

            _boardMembers = updatedMembers;

            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleMemberRemoved(Guid userId)
    {
        if (_currentUserId == userId)
        {
            var boardName = BoardStateService.CurrentBoardName;
            var message = string.IsNullOrWhiteSpace(boardName)
                ? "You have been removed from this board."
                : $"You have been removed from '{boardName}'.";

            InvokeAsync(() =>
            {
                Snackbar.Add(message, Severity.Warning);
                BoardStateService.NotifyBoardsListChanged();
                NavigationManager.NavigateTo("/");
            });
            return;
        }

        if (_boardMembers != null)
        {
            var memberToRemove = _boardMembers.FirstOrDefault(m => m.Dto.UserId == userId);

            if (memberToRemove != null)
            {
                _boardMembers.Remove(memberToRemove);
                InvokeAsync(StateHasChanged);
            }
        }
    }

    private void HandleCardMoved(CardMovedDto data)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == data.CardId);

        if (task != null)
        {
            task.Status = data.NewSectionName;
            task.SectionId = data.NewSectionId;
            task.Position = data.NewPosition;

            _tasks = _tasks.OrderBy(t => t.Position).ToList();

            InvokeAsync(() =>
            {
                StateHasChanged();
                _dropContainer?.Refresh();
            });
        }
    }

    private void HandleCardDeleted(Guid cardId)
    {
        var taskToRemove = _tasks.FirstOrDefault(t => t.Id == cardId);

        if (taskToRemove != null)
        {
            _tasks.Remove(taskToRemove);

            InvokeAsync(() =>
            {
                StateHasChanged();
                _dropContainer.Refresh();
            });
        }
    }

    private void HandleCardRenamed(CardRenameDto data)
    {
        var taskToUpdate = _tasks.FirstOrDefault(t => t.Id == data.CardId);

        if (taskToUpdate != null)
        {
            taskToUpdate.Name = data.NewTitle; 

            InvokeAsync(() =>
            {
                StateHasChanged();
                _dropContainer?.Refresh();
            });
        }
    }

    private void HandleCardDueDateUpdated(CardDueDateUpdateDto data)
    {
    }

    public async ValueTask DisposeAsync()
    {
        BoardStateService.OnBoardNameChanged -= StateHasChanged;
        BoardHubService.OnCardCreated -= HandleCardCreated;
        BoardHubService.OnSectionCreated -= HandleSectionCreated;
        BoardHubService.OnSectionRenamed -= HandleSectionRenamed;
        BoardHubService.OnSectionDeleted -= HandleSectionDeleted;
        BoardHubService.OnSectionMoved -= HandleSectionMoved;
        BoardHubService.OnMemberRoleUpdated -= HandleMemberRoleUpdated;
        BoardHubService.OnMemberRemoved -= HandleMemberRemoved;
        BoardHubService.OnCardMoved -= HandleCardMoved;
        BoardHubService.OnCardDeleted -= HandleCardDeleted;
        BoardHubService.OnCardRenamed -= HandleCardRenamed;
        BoardHubService.OnCardDueDateUpdated -= HandleCardDueDateUpdated;


        await BoardHubService.StopConnectionAsync(BoardIdGuid);
    }

}