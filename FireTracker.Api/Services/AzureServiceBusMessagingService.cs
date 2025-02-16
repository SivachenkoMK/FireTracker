using Azure.Messaging.ServiceBus;
using FireTracker.Api.Options;
using FireTracker.Api.Services.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FireTracker.Api.Services;

public class AzureServiceBusMessagingService : IMessagingService
{
    private readonly ServiceBusClient? _client;
    private readonly ServiceBusSender? _sender;

    public AzureServiceBusMessagingService(IOptions<AzureServiceBusConfiguration> options)
    {
        _client = new ServiceBusClient(options.Value.ConnectionString);
        _sender = _client.CreateSender(options.Value.TopicName);
    }

    public async Task PublishAsync(string routingKey, object message, CancellationToken cancellationToken)
    {
        var messageBody = JsonConvert.SerializeObject(message);
        var serviceBusMessage = new ServiceBusMessage(messageBody)
        {
            ApplicationProperties =
            {
                ["routingKey"] = routingKey
            }
        };

        if (_sender == null)
            throw new ArgumentNullException(nameof(_sender));
        
        await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_sender != null)
            await _sender.DisposeAsync();
        if (_client != null)
            await _client.DisposeAsync();
    }
}
