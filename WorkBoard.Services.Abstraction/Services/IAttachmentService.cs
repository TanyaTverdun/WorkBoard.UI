using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Abstraction.Services;

public interface IAttachmentService
{
    Task<IEnumerable<AttachmentDto>> GetAttachmentsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}
