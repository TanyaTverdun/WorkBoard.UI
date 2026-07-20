using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.UI.Components.Card;

public partial class CardLabelsSection : ComponentBase, IDisposable
{
    [Parameter, EditorRequired]
    public Guid CardId { get; set; }

    [Parameter, EditorRequired]
    public Guid BoardId { get; set; }

    [Parameter]
    public List<LabelDto> AppliedLabels { get; set; } = new();

    [Parameter]
    public EventCallback<List<LabelDto>> AppliedLabelsChanged { get; set; }

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    [Inject]
    private ILabelService LabelService { get; set; } = default!;

    private List<LabelDto> _allAvailableLabels = new();
    private List<LabelDto> _labels = new();
    private MudColor _newLabelColor = new("#4a4388ff");

    private bool _isLabelPopoverOpen = false;
    private string _labelSearchText = string.Empty;
    private bool _isCreatingNewLabel = false;
    private string _newLabelName = string.Empty;
    private Guid? _pendingDeleteLabelId = null;
    private Guid? _editingLabelId = null;
    private string _editingLabelName = string.Empty;
    private MudColor _editingLabelColor = new("#4a4388ff");

    protected override async Task OnInitializedAsync()
    {
        BoardHubService.OnLabelAddedToCard += HandleLabelAddedToCard;
        BoardHubService.OnLabelRemovedFromCard += HandleLabelRemovedFromCard;
        BoardHubService.OnLabelCreated += HandleLabelCreated;
        BoardHubService.OnLabelUpdated += HandleLabelUpdated;
        BoardHubService.OnLabelDeleted += HandleLabelDeleted;

        await LoadBoardLabelsAsync();
    }

    protected override void OnParametersSet()
    {
        _labels = AppliedLabels.ToList();
    }

    private async Task LoadBoardLabelsAsync()
    {
        try
        {
            var lables = await LabelService.GetLabelsByBoardAsync(BoardId);
            _allAvailableLabels = lables.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task ToggleLabelAsync(LabelDto label)
    {
        var existingLabel = _labels.FirstOrDefault(x => x.Id == label.Id);
        if (existingLabel != null)
        {
            try
            {
                await LabelService.RemoveLabelFromCardAsync(CardId, label.Id);
                _labels.Remove(existingLabel);
                await AppliedLabelsChanged.InvokeAsync(_labels);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing label: {ex.Message}");
            }
        }
        else
        {
            try
            {
                await LabelService.AddLabelToCardAsync(CardId, label.Id);
                _labels.Add(label);
                await AppliedLabelsChanged.InvokeAsync(_labels);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error attaching label: {ex.Message}");
            }
        }
        StateHasChanged();
    }

    private IEnumerable<LabelDto> FilteredLabels =>
        string.IsNullOrWhiteSpace(_labelSearchText)
            ? _allAvailableLabels
            : _allAvailableLabels.Where(l => l.Name.Contains(
                _labelSearchText, StringComparison.OrdinalIgnoreCase));

    private void InitiateDeleteLabel(Guid labelId)
    {
        _pendingDeleteLabelId = labelId;
    }

    private void CancelDeleteLabel()
    {
        _pendingDeleteLabelId = null;
    }

    private async Task ConfirmDeleteLabelAsync(LabelDto label)
    {
        try
        {
            await LabelService.DeleteLabelAsync(label.Id);

            _allAvailableLabels.RemoveAll(l => l.Id == label.Id);
            _labels.RemoveAll(l => l.Id == label.Id);

            _pendingDeleteLabelId = null;
            await AppliedLabelsChanged.InvokeAsync(_labels);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting label: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
        }
    }

    private void EditLabel(LabelDto label)
    {
        _editingLabelId = label.Id;
        _editingLabelName = label.Name;
        _editingLabelColor = new MudColor(label.Color ?? "#4a4388ff");
    }

    private void CancelEditLabel()
    {
        _editingLabelId = null;
    }

    private async Task SaveEditLabelAsync()
    {
        if (string.IsNullOrWhiteSpace(_editingLabelName) || 
            _editingLabelId == null)
        {
            return;
        }

        try
        {
            var request = new UpdateLabelRequest(_editingLabelName, _editingLabelColor.Value);

            await LabelService.UpdateLabelAsync(_editingLabelId.Value, request);

            var label = _allAvailableLabels.FirstOrDefault(l => l.Id == _editingLabelId);
            if (label != null)
            {
                label.Name = _editingLabelName;
                label.Color = _editingLabelColor.Value;
            }

            _editingLabelId = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating label: {ex.Message}");
        }
    }

    private void ShowCreateLabelForm()
    {
        _isCreatingNewLabel = true;
        _newLabelName = string.Empty;
        _newLabelColor = new("#4a4388ff");
    }

    private void HideCreateLabelForm()
    {
        _isCreatingNewLabel = false;
        _newLabelName = string.Empty;
    }

    private async Task CreateNewLabelAsync()
    {
        if (string.IsNullOrWhiteSpace(_newLabelName))
        {
            return;
        }

        var trimmedName = _newLabelName.Trim();

        if (_allAvailableLabels.Any(
            l => l.Name.Equals(trimmedName, StringComparison.OrdinalIgnoreCase)))
        {
            HideCreateLabelForm();
            return;
        }

        try
        {
            var request = new CreateLabelRequest
            {
                Name = trimmedName,
                Color = _newLabelColor.Value
            };

            var newLabel = await LabelService.CreateLabelAsync(
                CardId,
                request);

            _allAvailableLabels.Add(newLabel);
            _labels.Add(newLabel);

            HideCreateLabelForm();

            await AppliedLabelsChanged.InvokeAsync(_labels);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating label: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
        }
    }

    private void HandleLabelAddedToCard(Guid cardId, LabelDto label)
    {
        if (CardId == cardId && 
            !_labels.Any(l => l.Id == label.Id))
        {
            _labels.Add(label);
            AppliedLabelsChanged.InvokeAsync(_labels);
            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleLabelRemovedFromCard(Guid cardId, Guid labelId)
    {
        if (CardId == cardId &&
                _labels.RemoveAll(l => l.Id == labelId) > 0)
        {
            AppliedLabelsChanged.InvokeAsync(_labels);
            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleLabelCreated(LabelDto newLabel)
    {
        if (!_allAvailableLabels.Any(l => l.Id == newLabel.Id))
        {
            _allAvailableLabels.Add(newLabel);
            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleLabelUpdated(LabelDto updatedLabel)
    {
        bool changed = false;

        var boardLabel = _allAvailableLabels.FirstOrDefault(l => l.Id == updatedLabel.Id);
        if (boardLabel != null)
        {
            boardLabel.Name = updatedLabel.Name;
            boardLabel.Color = updatedLabel.Color;
            changed = true;
        }

        var cardLabel = _labels.FirstOrDefault(l => l.Id == updatedLabel.Id);
        if (cardLabel != null)
        {
            cardLabel.Name = updatedLabel.Name;
            cardLabel.Color = updatedLabel.Color;
            changed = true;
        }

        if (changed)
        {
            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleLabelDeleted(Guid labelId)
    {
        bool changed = false;

        if (_allAvailableLabels.RemoveAll(l => l.Id == labelId) > 0)
        {
            changed = true;
        }

        if (_labels.RemoveAll(l => l.Id == labelId) > 0)
        {
            changed = true;
            AppliedLabelsChanged.InvokeAsync(_labels);
        }

        if (changed)
        {
            InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        BoardHubService.OnLabelAddedToCard -= HandleLabelAddedToCard;
        BoardHubService.OnLabelRemovedFromCard -= HandleLabelRemovedFromCard;
        BoardHubService.OnLabelCreated -= HandleLabelCreated;
        BoardHubService.OnLabelUpdated -= HandleLabelUpdated;
        BoardHubService.OnLabelDeleted -= HandleLabelDeleted;
    }
}
