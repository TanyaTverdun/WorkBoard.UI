using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.UI.Pages;

public partial class BoardPage
{
    [Inject]
    private ISectionService SectionService { get; set; }

    [Parameter]
    public Guid BoardIdGuid { get; set; }

    private MudDropContainer<KanbanTaskItem> _dropContainer;
    private bool _addSectionOpen;

    private List<KanBanSections> _sections = new();
    private List<KanbanTaskItem> _tasks = new();

    private KanBanNewForm newSectionModel = new KanBanNewForm();

    protected override async Task OnParametersSetAsync()
    {
        var sectionsFromDb = await SectionService
            .GetSectionsByBoardAsync(BoardIdGuid);

        _sections = sectionsFromDb.Select(s =>
            new KanBanSections(s.Id, s.Name, false, string.Empty)
        ).ToList();
    }

    private void TaskUpdated(MudItemDropInfo<KanbanTaskItem> info)
    {
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

        var newSection = new KanBanSections(
            newSectionId,
            newSectionModel.Name,
            false,
            string.Empty);

        _sections.Add(newSection);

        newSectionModel.Name = string.Empty;
        _addSectionOpen = false;
    }

    private async Task SaveRename(KanBanSections section)
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

    private async Task DeleteSection(KanBanSections section)
    {
        await SectionService.DeleteSectionAsync(
            BoardIdGuid, 
            section.Id);

        _sections.Remove(section);
        _tasks.RemoveAll(t => t.Status == section.Name);

        _dropContainer.Refresh();
    }

    private void StartRename(KanBanSections section)
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

    private void AddTask(KanBanSections section)
    {
        _tasks.Add(new KanbanTaskItem(
            section.NewTaskName, 
            section.Name));

        section.NewTaskName = string.Empty;
        section.NewTaskOpen = false;
        _dropContainer.Refresh();
    }

    private void CloseNewTaskForm(KanBanSections section)
    {
        section.NewTaskOpen = false;
        section.NewTaskName = string.Empty;
    }

    public class KanBanSections
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public bool NewTaskOpen { get; set; }
        public string NewTaskName { get; set; }
        public bool IsConfirmingDelete { get; set; }
        public bool IsRenaming { get; set; }
        public string EditName { get; set; }

        private bool _menuOpen;
        public bool MenuOpen
        {
            get => _menuOpen;
            set
            {
                _menuOpen = value;
                if (!value) IsConfirmingDelete = false;
            }
        }

        public KanBanSections(
            Guid id, 
            string name, 
            bool newTaskOpen, 
            string newTaskName)
        {
            Id = id;
            Name = name;
            NewTaskOpen = newTaskOpen;
            NewTaskName = newTaskName;
            IsConfirmingDelete = false;
            IsRenaming = false;
        }
    }

    public class KanbanTaskItem
    {
        public string Name { get; init; }
        public string Status { get; set; }

        public KanbanTaskItem(
            string name, 
            string status)
        {
            Name = name;
            Status = status;
        }
    }

    public class KanBanNewForm
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 50.")]
        public string Name { get; set; }
    }
}
