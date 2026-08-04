namespace WorkBoard.Domain.Enums;

public enum BoardArchiveStatus : byte
{
    Active = 0,
    Pending = 1,
    Queued = 2,
    Archived = 3,
    RestorePending = 4
}
