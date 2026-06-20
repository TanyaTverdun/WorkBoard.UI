using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.UI.ViewModels.Board;

namespace WorkBoard.UI.Pages;

public partial class BoardPage
{
    [Inject]
    private ISectionService SectionService { get; set; } = default!;

    [Parameter]
    public Guid BoardIdGuid { get; set; }

    private MudDropContainer<KanbanTaskViewModel> _dropContainer = default!;
    private bool _addSectionOpen;

    private List<KanbanSectionViewModel> _sections = new();
    private List<KanbanTaskViewModel> _tasks = new();

    private CreateSectionForm newSectionModel = new CreateSectionForm();

    private bool _isReorderPopoverOpen;
    private List<KanbanSectionViewModel> _reorderList = new();

    protected override async Task OnParametersSetAsync()
    {
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
}
