using FireTracker.Analysis.DTOs;
using FireTracker.Analysis.Services.Abstractions;

namespace FireTracker.Analysis.Services;

public class RoutingService
{
    private readonly IMessagingPublisher _messagingPublisher;

    public RoutingService(IMessagingPublisher messagingPublisher)
    {
        _messagingPublisher = messagingPublisher;
    }

    public async Task SendAnalysis(AnalysisResult request, CancellationToken token)
    {
        const string routingKey = "fire.analysis";
        await _messagingPublisher.PublishAsync(routingKey, request, token);
    }
}