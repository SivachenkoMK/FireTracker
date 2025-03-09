using FireTracker.Core.DTOs;
using FireTracker.Core.Services;
using FireTracker.Core.Services.Abstractions;
using Newtonsoft.Json;

namespace FireTracker.Core.Background;

public class ConsumerWorker : BackgroundService
{
    private readonly IMessagingConsumer _messagingConsumer;
    private readonly ILogger<ConsumerWorker> _logger;
    private readonly IncidentService _incidentService;

    public ConsumerWorker(IMessagingConsumer messagingConsumer, ILogger<ConsumerWorker> logger, IncidentService incidentService)
    {
        _messagingConsumer = messagingConsumer;
        _logger = logger;
        _incidentService = incidentService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messagingConsumer.OnMessageReceivedAsync += (message, routingKey) =>
        {
            // Process the message based on queue name
            if (routingKey == "fire.location")
            {
                var locationInformation = JsonConvert.DeserializeObject<LocationRequest>(message);
                if (locationInformation == null)
                {
                    _logger.LogError("Received message: {Message} for location queue, but couldn't deserialize it", message);
                    return Task.CompletedTask;
                }
                
                _incidentService.UpdateLocation(locationInformation);
            }
            else if (routingKey == "fire.gis")
            {
                var gisRequest = JsonConvert.DeserializeObject<GisRequest>(message);
                if (gisRequest == null)
                {
                    _logger.LogError("Received message: {Message} for gis queue, but couldn't deserialize it", message);
                    return Task.CompletedTask;
                }
                
                _incidentService.UpdateGis(gisRequest);
            }
            else if (routingKey == "fire.analysis")
            {
                var analysisResult = JsonConvert.DeserializeObject<AnalysisResult>(message);
                if (analysisResult == null)
                {
                    _logger.LogError("Received message: {Message} for analysis queue, but couldn't deserialize it", message);
                    return Task.CompletedTask;
                }
                
                _incidentService.UpdateAnalysis(analysisResult);
            }
            else
            {
                _logger.LogWarning("Received message: {Message} for unknown routing key {Queue}", message, routingKey);
            }

            return Task.CompletedTask;
        };

        await _messagingConsumer.StartConsumingAsync(stoppingToken); 
    }
}