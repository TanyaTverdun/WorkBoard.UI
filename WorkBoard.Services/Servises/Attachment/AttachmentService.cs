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

    public async Task<IEnumerable<AttachmentDto>> GetAttachmentsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _attachmentApi.GetAttachmentsByCardAsync(
                cardId, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx, 
                "API error getting attachments for card {CardId}. " +
                "Status: {StatusCode}", 
                cardId, 
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error getting attachments for card {CardId}", 
                cardId);
            throw;
        }
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
}
