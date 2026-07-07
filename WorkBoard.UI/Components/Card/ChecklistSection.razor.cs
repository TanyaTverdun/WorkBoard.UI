using Microsoft.AspNetCore.Components;
using MudBlazor;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.UI.Components.Card;

public partial class ChecklistSection : ComponentBase
{
    [Parameter]
    public Guid CardId { get; set; }

    [Inject]
    private IChecklistService ChecklistService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private ChecklistDto? _checklist;
    private bool _isHoveringChecklistTitle = false;
    private bool _isEditingChecklistTitle = false;
    private string _editedChecklistTitle = string.Empty;
    private bool _isPendingDeleteChecklist = false;

    private bool _isAddingChecklistItem = false;
    private string _newChecklistItemTitle = string.Empty;
    private Guid? _hoveredItemId = null;
    private Guid? _editingItemId = null;
    private string _editedItemTitle = string.Empty;
    private Guid? _pendingDeleteChecklistItemId = null;

    private int CompletedChecklistItems => _checklist?.Items?.Count(x => x.IsDone) ?? 0;
    private int TotalChecklistItems => _checklist?.Items?.Count ?? 0;
    private double ChecklistProgress => TotalChecklistItems == 0 ? 0
        : Math.Round((double)CompletedChecklistItems / TotalChecklistItems * 100);

    protected override async Task OnInitializedAsync()
    {
        await LoadChecklistAsync();
    }

    private async Task LoadChecklistAsync()
    {
        try
        {
            _checklist = await ChecklistService.GetChecklistByCardAsync(CardId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading checklist: {ex.Message}");
        }
    }

    private void StartAddingChecklist()
    {
        _checklist = null;
        _editedChecklistTitle = string.Empty;
        _isEditingChecklistTitle = true;
        _isPendingDeleteChecklist = false;
    }

    private void EnableChecklistTitleEdit()
    {
        _editedChecklistTitle = _checklist?.Name ?? string.Empty;
        _isEditingChecklistTitle = true;
        _isPendingDeleteChecklist = false;
    }

    private async Task SaveChecklistTitle()
    {
        if (string.IsNullOrWhiteSpace(_editedChecklistTitle))
        {
            _isEditingChecklistTitle = false;
            return;
        }

        var trimmedTitle = _editedChecklistTitle.Trim();

        if (_checklist != null && trimmedTitle == _checklist.Name)
        {
            _isEditingChecklistTitle = false;
            return;
        }

        try
        {
            if (_checklist != null)
            {
                var request = new UpdateChecklistRequest 
                { 
                    Name = trimmedTitle 
                };

                _checklist = await ChecklistService.UpdateChecklistAsync(
                    _checklist.ChecklistId, 
                    request);
            }
            else
            {
                var request = new CreateChecklistRequest 
                { 
                    Name = trimmedTitle 
                };

                _checklist = await ChecklistService.CreateChecklistAsync(
                    CardId, 
                    request);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving checklist title: {ex.Message}");
            Snackbar.Add("Failed to save checklist.", Severity.Error);
        }
        finally
        {
            _isEditingChecklistTitle = false;
            StateHasChanged();
        }
    }

    private void CancelChecklistTitleEdit()
    {
        _isEditingChecklistTitle = false;
    }

    private async Task ConfirmDeleteChecklist()
    {
        if (_checklist != null)
        {
            try
            {
                await ChecklistService.DeleteChecklistAsync(
                    _checklist.ChecklistId);

                _checklist = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting checklist: {ex.Message}");
                _isPendingDeleteChecklist = false;
                StateHasChanged();
                return;
            }
        }

        _isPendingDeleteChecklist = false;
        _isHoveringChecklistTitle = false;
        StateHasChanged();
    }

    private void ShowAddChecklistItemForm()
    {
        _isAddingChecklistItem = true;
        _newChecklistItemTitle = string.Empty;
    }

    private void CancelAddChecklistItem()
    {
        _isAddingChecklistItem = false;
        _newChecklistItemTitle = string.Empty;
    }

    private async Task AddChecklistItem()
    {
        if (string.IsNullOrWhiteSpace(_newChecklistItemTitle)
            || _checklist == null)
        {
            return;
        }

        var titleToSave = _newChecklistItemTitle.Trim();

        if (_checklist.Items != null && 
            _checklist.Items.Any(x => x.Title.Equals(
                titleToSave, 
                StringComparison.OrdinalIgnoreCase)))
        {
            Snackbar.Add(
                "Item with this title already exists in the checklist.", 
                Severity.Warning);

            return;
        }

        try
        {
            var request = new AddChecklistItemRequest 
            { 
                Title = titleToSave 
            };

            var newItem = await ChecklistService.AddChecklistItemAsync(
                _checklist.ChecklistId, 
                request);

            var currentItems = _checklist.Items?.ToList() ?? new List<ChecklistItemDto>();
            currentItems.Add(newItem);
            _checklist.Items = currentItems;

            _newChecklistItemTitle = string.Empty;
            _isAddingChecklistItem = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding checklist item: {ex.Message}");
            Snackbar.Add("Failed to add checklist item.", Severity.Error);
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task UpdateItemStatusAsync(ChecklistItemDto item, bool isDone)
    {
        item.IsDone = isDone;
        StateHasChanged();

        try
        {
            var request = new UpdateChecklistItemStatusRequest 
            { 
                IsDone = isDone 
            };

            var updatedItem = await ChecklistService.UpdateChecklistItemStatusAsync(
                item.Id, 
                request);

            item.IsDone = updatedItem.IsDone;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating item status: {ex.Message}");
            item.IsDone = !isDone;
            Snackbar.Add("Failed to update item status. Please try again.", Severity.Error);
        }
        finally
        {
            StateHasChanged();
        }
    }

    private void EnableItemEdit(ChecklistItemDto item)
    {
        _editingItemId = item.Id;
        _editedItemTitle = item.Title;
        _hoveredItemId = null;
    }

    private void CancelItemEdit()
    {
        _editingItemId = null;
        _editedItemTitle = string.Empty;
    }

    private async Task SaveItemTitleAsync(ChecklistItemDto item)
    {
        if (string.IsNullOrWhiteSpace(_editedItemTitle))
        {
            CancelItemEdit();
            return;
        }

        var trimmedTitle = _editedItemTitle.Trim();

        if (trimmedTitle == item.Title)
        {
            CancelItemEdit();
            return;
        }

        try
        {
            var request = new UpdateChecklistItemRequest 
            { 
                Title = trimmedTitle 
            };

            var updatedItem = await ChecklistService.UpdateChecklistItemAsync(
                item.Id, 
                request);

            item.Title = updatedItem.Title;
            CancelItemEdit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating item title: {ex.Message}");
            Snackbar.Add(
                "Failed to update item title or this title already exists.", 
                Severity.Error);
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task DeleteChecklistItemAsync(ChecklistItemDto item)
    {
        try
        {
            await ChecklistService.DeleteChecklistItemAsync(item.Id);

            if (_checklist?.Items != null)
            {
                var currentItems = _checklist.Items.ToList();
                var itemToRemove = currentItems.FirstOrDefault(
                    x => x.Id == item.Id);

                if (itemToRemove != null)
                {
                    currentItems.Remove(itemToRemove);
                    _checklist.Items = currentItems;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting checklist item: {ex.Message}");
            Snackbar.Add("Failed to delete item. Please try again.", Severity.Error);
        }
        finally
        {
            _pendingDeleteChecklistItemId = null;
            StateHasChanged();
        }
    }
}
