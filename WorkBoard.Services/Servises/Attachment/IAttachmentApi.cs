using Refit;
using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Servises.Attachment;

public interface IAttachmentApi
{
    [Get("/api/cards/{cardId}/attachments")]
    Task<IEnumerable<AttachmentDto>> GetAttachmentsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}
