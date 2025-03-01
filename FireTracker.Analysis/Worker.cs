using System.Text.Json;
using FireTracker.Analysis.DTOs;
using FireTracker.Analysis.Services;
using FireTracker.Analysis.Services.Abstractions;

namespace FireTracker.Analysis;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMessagingConsumer _messagingConsumer;
    private readonly ImageProcessor _imageProcessing;
    private readonly RoutingService _routingService;
    private readonly InterpretationService _interpretationService;

    public Worker(ILogger<Worker> logger,
        IMessagingConsumer messagingConsumer,
        ImageProcessor imageProcessing,
        RoutingService routingService,
        InterpretationService interpretationService)
    {
        _logger = logger;
        _messagingConsumer = messagingConsumer;
        _imageProcessing = imageProcessing;
        _routingService = routingService;
        _interpretationService = interpretationService;
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

            var analysisResult = new AnalysisResult
            {
                SessionId = sessionModel.SessionGuid,
                Detection = _interpretationService.InterpretAnalysedResult(result)
            };

            await _routingService.SendAnalysis(analysisResult, stoppingToken);
            _logger.LogInformation("Analysis result: {Raw}, {Result}", result, analysisResult.Detection);
        };

        await _messagingConsumer.StartConsumingAsync(stoppingToken);
    }
}