using System.ComponentModel.DataAnnotations;

namespace WorkBoard.Services.Abstraction.Requests;

public class UpdateSectionNameRequest
{
    [Required(ErrorMessage = "Section name is required")]
    [StringLength(50, ErrorMessage = "Section name must not exceed 50 characters")]
    public string Name { get; set; } = string.Empty;
}
