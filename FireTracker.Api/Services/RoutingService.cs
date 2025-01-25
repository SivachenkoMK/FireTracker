using FireTracker.Api.DTOs;

namespace FireTracker.Api.Services;

public class RoutingService
{
    private readonly MessagingService _messagingService;
    private readonly StorageService _storageService;

    public RoutingService(MessagingService messagingService, StorageService storageService)
    {
        _messagingService = messagingService;
        _storageService = storageService;
    }

    public async Task SendGisRequest(GisRequest request, CancellationToken token)
    {
        const string routingKey = "fire.gis";
        await _messagingService.PublishToRabbitMq(routingKey, request, token);
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

        await _messagingService.PublishToRabbitMq(routingKey, photoInfo, token);
    }

    public async Task SendRelationalLocation(LocationRequest request, CancellationToken token)
    {
        const string routingKey = "fire.location";
        await _messagingService.PublishToRabbitMq(routingKey, request, token);
    }
}