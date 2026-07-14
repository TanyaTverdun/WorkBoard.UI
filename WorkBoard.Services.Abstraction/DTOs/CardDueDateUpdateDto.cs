namespace WorkBoard.Services.Abstraction.DTOs;

public class CardDueDateUpdateDto
{
    public Guid CardId { get; set; }
    public DateTime? DueDate { get; set; }
}
