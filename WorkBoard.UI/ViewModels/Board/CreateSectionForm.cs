using System.ComponentModel.DataAnnotations;

namespace WorkBoard.UI.ViewModels.Board;

public class CreateSectionForm
{
    [Required]
    [StringLength(50, ErrorMessage = "Name length can't be more than 50.")]
    public string Name { get; set; } = String.Empty;
}
