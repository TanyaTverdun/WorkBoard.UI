using System.ComponentModel.DataAnnotations;

namespace WorkBoard.Services.Abstraction.Requests.Sections;

public class CreateSectionRequest
{
    [Required(ErrorMessage = "Section name is required")]
    [StringLength(50, ErrorMessage = "Section name must not exceed 50 characters")]
    public string Name { get; set; } = string.Empty;
}
