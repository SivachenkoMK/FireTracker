using Azure.Messaging.ServiceBus;
using FireTracker.Analysis.Options;
using FireTracker.Analysis.Services.Abstractions;
using Microsoft.Extensions.Options;

namespace FireTracker.Analysis.Services;

public class AzureServiceBusMessagingConsumer : IMessagingConsumer
{
    private readonly ILogger<AzureServiceBusMessagingConsumer> _logger;
    private readonly AzureServiceBusConfiguration _configuration;
    private readonly ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    public event Func<string, Task>? OnMessageReceivedAsync;

    public AzureServiceBusMessagingConsumer(ILogger<AzureServiceBusMessagingConsumer> logger, IOptions<AzureServiceBusConfiguration> configuration)
    {
        _logger = logger;
        _configuration = configuration.Value;
        _client = new ServiceBusClient(_configuration.ConnectionString);
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        if (_client == null)
            throw new ArgumentNullException(nameof(_client), "Couldn't connect to Azure Service Bus");
        
        _processor = _client.CreateProcessor(_configuration.TopicName, _configuration.SubscriptionName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        });

        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(cancellationToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            var messageBody = args.Message.Body.ToString();
            _logger.LogInformation("ASB Received message: {Message}", messageBody);

            if (OnMessageReceivedAsync != null)
            {
                await OnMessageReceivedAsync.Invoke(messageBody);
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
        if (_processor is not null)
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
        }

        if (_client is not null)
        {
            await _client.DisposeAsync();
        }
    }
}