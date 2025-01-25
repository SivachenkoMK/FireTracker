using System.Text.Json;

namespace FireTracker.Analysis;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly MessageConsumer _messageConsumer;
    private readonly ImageProcessor _imageProcessing;

    public Worker(ILogger<Worker> logger,
        MessageConsumer messageConsumer,
        ImageProcessor imageProcessing)
    {
        _logger = logger;
        _messageConsumer = messageConsumer;
        _imageProcessing = imageProcessing;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageConsumer.OnMessageReceivedAsync += async sessionInformation => 
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

        await _messageConsumer.StartConsumingAsync(stoppingToken);
    }
}