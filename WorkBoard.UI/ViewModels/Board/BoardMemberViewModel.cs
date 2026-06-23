using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.UI.ViewModels.Board;

public class BoardMemberViewModel
{
    public BoardMemberDto Dto { get; set; }

    public bool IsConfirmingDelete { get; set; }

    public BoardMemberViewModel(BoardMemberDto dto)
    {
        Dto = dto;
    }
}
