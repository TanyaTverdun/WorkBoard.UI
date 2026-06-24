using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using WorkBoard.Domain.Enums;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;
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

        var newSectionId = await SectionService.CreateSectionAsync(
            BoardIdGuid,
            request);

        double newPos = _sections.Any() ?
            _sections.Max(s => s.Position) + 1.0 : 1.0;

        var newSection = new KanbanSectionViewModel(
            newSectionId,
            newSectionModel.Name,
            false,
            string.Empty,
            newPos);

        _sections.Add(newSection);

        newSectionModel.Name = string.Empty;
        _addSectionOpen = false;
    }

    private async Task SaveRename(KanbanSectionViewModel section)
    {
        if (string.IsNullOrWhiteSpace(section.EditName))
        {
            section.IsRenaming = false;
            return;
        }

        var newName = section.EditName.Trim();
        var request = new UpdateSectionNameRequest
        {
            Name = newName
        };

        await SectionService.RenameSectionAsync(
            BoardIdGuid,
            section.Id,
            request);

        string oldName = section.Name;
        section.Name = newName;

        var tasksToUpdate = _tasks
            .Where(t => t.Status == oldName)
            .ToList();

        foreach (var t in tasksToUpdate)
        {
            t.Status = newName;
        }

        section.IsRenaming = false;
        _dropContainer.Refresh();
    }

    private async Task DeleteSection(KanbanSectionViewModel section)
    {
        await SectionService.DeleteSectionAsync(
            BoardIdGuid,
            section.Id);

        _sections.Remove(section);
        _tasks.RemoveAll(
            t => t.Status == section.Name);

        _dropContainer.Refresh();
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

    private void AddTask(KanbanSectionViewModel section)
    {
        _tasks.Add(new KanbanTaskViewModel(
            section.NewTaskName,
            section.Name));

        section.NewTaskName = string.Empty;
        section.NewTaskOpen = false;
        _dropContainer.Refresh();
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

    private async Task UpdateRoleAsync(BoardMemberViewModel member, BoardRole newRole)
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
}