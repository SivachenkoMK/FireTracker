using System.Text.Json;
using Azure.Messaging.ServiceBus.Administration;
using FireTracker.Analysis.Options;
using FireTracker.Analysis.Services.Abstractions;
using Microsoft.Extensions.Options;

namespace FireTracker.Analysis;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMessagingConsumer _messagingConsumer;
    private readonly ImageProcessor _imageProcessing;
    private readonly AzureServiceBusConfiguration _configuration;

    public Worker(ILogger<Worker> logger,
        IMessagingConsumer messagingConsumer,
        ImageProcessor imageProcessing,
        IOptions<AzureServiceBusConfiguration> azureServiceBusConfiguration)
    {
        _logger = logger;
        _messagingConsumer = messagingConsumer;
        _imageProcessing = imageProcessing;
        _configuration = azureServiceBusConfiguration.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RemoveDefaultRuleFromSubscription(stoppingToken);

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

    private async Task RemoveDefaultRuleFromSubscription(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(_configuration.ConnectionString))
            return; // Must be running in local environment then, ASB not initialized
        
        var adminClient = new ServiceBusAdministrationClient(_configuration.ConnectionString);

        // Remove the default rule if it exists (comes from bicep configuration, so should be careful if not auto-removed)
        if (await adminClient.RuleExistsAsync(_configuration.TopicName, _configuration.SubscriptionName, "$Default", stoppingToken))
        {
            _logger.LogInformation("Deleting the '$Default' rule from subscription '{Subscription}'", _configuration.SubscriptionName);
            await adminClient.DeleteRuleAsync(_configuration.TopicName, _configuration.SubscriptionName, "$Default", stoppingToken);
        }
    }
}