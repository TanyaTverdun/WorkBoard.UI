using System.ComponentModel.DataAnnotations;

namespace WorkBoard.Services.Abstraction.Requests;

public class UpdateBoardRequest
{
    [Required(ErrorMessage = "Board name is required.")]
    [StringLength(50, ErrorMessage = "Board name must not exceed 50 characters.")]
    public string Name { get; set; } = string.Empty;
}
