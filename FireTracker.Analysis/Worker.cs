using System.Text.Json;
using FireTracker.Analysis.Services.Abstractions;

namespace FireTracker.Analysis;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMessagingConsumer _messagingConsumer;
    private readonly ImageProcessor _imageProcessing;

    public Worker(ILogger<Worker> logger,
        IMessagingConsumer messagingConsumer,
        ImageProcessor imageProcessing)
    {
        _logger = logger;
        _messagingConsumer = messagingConsumer;
        _imageProcessing = imageProcessing;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messagingConsumer.OnMessageReceivedAsync += async sessionInformation => 
        {       
            var sessionModel = JsonSerializer.Deserialize<SessionInformationModel>(sessionInformation);
            if (sessionModel == null)
            {
                _logger.LogError("Invalid session information");
                return;
            }

            var result = await _imageProcessing.ProcessImageAsync(sessionModel);

            _logger.LogInformation($"ONNX result: {result}");
        };

        await _messagingConsumer.StartConsumingAsync(stoppingToken);
    }
}