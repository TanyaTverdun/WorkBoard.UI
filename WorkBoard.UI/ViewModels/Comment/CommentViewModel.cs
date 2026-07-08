namespace WorkBoard.UI.ViewModels.Comment;

public class CommentViewModel
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Text { get; set; } = string.Empty;
}
