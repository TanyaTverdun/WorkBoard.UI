using Refit;
using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Abstraction.Services;

public interface IAttachmentService
{
    Task<IEnumerable<AttachmentDto>> GetAttachmentsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);

    Task<AttachmentDto> UploadAttachmentAsync(
        Guid cardId, 
        StreamPart file, 
        CancellationToken cancellationToken = default);

    Task DeleteAttachmentAsync(
        Guid cardId,
        Guid attachmentId,
        CancellationToken cancellationToken = default);
}
