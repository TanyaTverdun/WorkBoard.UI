namespace WorkBoard.Services.Abstraction.DTOs.Cards;

public class CardDueDateUpdateDto
{
    public Guid CardId { get; set; }
    public DateTime? DueDate { get; set; }
}
