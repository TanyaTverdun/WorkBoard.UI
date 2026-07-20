using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Attachment;

public class AttachmentService : IAttachmentService
{
    private readonly IAttachmentApi _attachmentApi;
    private readonly ILogger<AttachmentService> _logger;

    public AttachmentService(
        IAttachmentApi attachmentApi, 
        ILogger<AttachmentService> logger)
    {
        _attachmentApi = attachmentApi;
        _logger = logger;
    }

    public async Task<AttachmentDto> UploadAttachmentAsync(
        Guid cardId, 
        StreamPart file, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _attachmentApi.UploadAttachmentAsync(
                cardId, 
                file, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx, 
                "API error uploading attachment for card {CardId}. " +
                "Status: {StatusCode}", 
                cardId, 
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error uploading attachment for card {CardId}", 
                cardId);
            throw;
        }
    }

    public async Task DeleteAttachmentAsync(
        Guid cardId,
        Guid attachmentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _attachmentApi.DeleteAttachmentAsync(
                cardId,
                attachmentId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error deleting attachment {AttachmentId}" +
                " for card {CardId}. Status: {StatusCode}",
                attachmentId, cardId, apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error deleting attachment {AttachmentId} for card {CardId}",
                attachmentId, cardId);
            throw;
        }
    }
}
