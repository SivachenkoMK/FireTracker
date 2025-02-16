using FireTracker.Api.DTOs;
using FireTracker.Api.Services.Abstractions;

namespace FireTracker.Api.Services;

public class RoutingService
{
    private readonly IMessagingService _messagingService;
    private readonly StorageService _storageService;

    public RoutingService(IMessagingService messagingService, StorageService storageService)
    {
        _messagingService = messagingService;
        _storageService = storageService;
    }

    public async Task SendGisRequest(GisRequest request, CancellationToken token)
    {
        const string routingKey = "fire.gis";
        await _messagingService.PublishAsync(routingKey, request, token);
    }

    public async Task SendPhotoRequest(PhotoRequest request, CancellationToken token)
    {
        const string routingKey = "fire.photo";
        var photoUrl = await _storageService.Upload(request.Photo, request.SessionGuid, token);

        var photoInfo = new
        {
            request.SessionGuid,
            PhotoUrl = photoUrl,
            PhotoLength = request.Photo.Length
        };

        await _messagingService.PublishAsync(routingKey, photoInfo, token);
    }

    public async Task SendRelationalLocation(LocationRequest request, CancellationToken token)
    {
        const string routingKey = "fire.location";
        await _messagingService.PublishAsync(routingKey, request, token);
    }
}