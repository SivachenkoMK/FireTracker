using Azure.Messaging.ServiceBus;
using FireTracker.Core.Options;
using FireTracker.Core.Services.Abstractions;
using Microsoft.Extensions.Options;

namespace FireTracker.Core.Services;

public class AzureServiceBusMessagingConsumer : IMessagingConsumer
{
    private readonly ILogger<AzureServiceBusMessagingConsumer> _logger;
    private readonly InboundQueueConfiguration _inboundQueueConfiguration;
    private readonly AzureServiceBusConfiguration _configuration;
    private readonly ServiceBusClient? _client;
    
    private ServiceBusProcessor? _analysisProcessor;
    private ServiceBusProcessor? _locationProcessor;

    public event Func<string, string, Task>? OnMessageReceivedAsync;

    public AzureServiceBusMessagingConsumer(ILogger<AzureServiceBusMessagingConsumer> logger, IOptions<AzureServiceBusConfiguration> configuration, IOptions<InboundQueueConfiguration> inboundQueueConfiguration)
    {
        _logger = logger;
        _inboundQueueConfiguration = inboundQueueConfiguration.Value;
        _configuration = configuration.Value;
        _client = new ServiceBusClient(_configuration.ConnectionString);
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        if (_client == null)
            throw new ArgumentNullException(nameof(_client), "Couldn't connect to Azure Service Bus");
        
        _analysisProcessor = _client.CreateProcessor(_configuration.TopicName, _inboundQueueConfiguration.AnalysisQueue, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        });

        _analysisProcessor.ProcessMessageAsync += MessageHandler;
        _analysisProcessor.ProcessErrorAsync += ErrorHandler;
        
        _locationProcessor = _client.CreateProcessor(_configuration.TopicName, _inboundQueueConfiguration.IncidentQueue, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        });
        
        _locationProcessor.ProcessMessageAsync += MessageHandler;
        _locationProcessor.ProcessErrorAsync += ErrorHandler;

        await _analysisProcessor.StartProcessingAsync(cancellationToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            var messageBody = args.Message.Body.ToString();
            var queueName = args.Message.ApplicationProperties["routingKey"].ToString() ?? "none";
            
            _logger.LogInformation("ASB Received message: {Message}", messageBody);

            if (OnMessageReceivedAsync != null)
            {
                await OnMessageReceivedAsync.Invoke(messageBody, queueName);
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ASB message");
            await args.DeadLetterMessageAsync(args.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "ASB error: {ErrorSource}", args.ErrorSource);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_analysisProcessor is not null)
        {
            await _analysisProcessor.StopProcessingAsync();
            await _analysisProcessor.DisposeAsync();
        }

        if (_client is not null)
        {
            await _client.DisposeAsync();
        }
    }
}