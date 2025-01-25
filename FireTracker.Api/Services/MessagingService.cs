using System.Text;
using FireTracker.Api.Options;
using FireTracker.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FireTracker.Api.Services;

public class MessagingService : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly AsyncLazy _initializationTask;
    private readonly MessageQueueConfiguration _messageQueueConfiguration;

    public MessagingService(IOptions<MessageQueueConfiguration> exchangeOptions)
    {
        _messageQueueConfiguration = exchangeOptions.Value;
        _initializationTask = new AsyncLazy(InitializeRabbitMqAsync);
    }

    private async Task InitializeRabbitMqAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _messageQueueConfiguration.HostName,
            UserName = _messageQueueConfiguration.UserName,
            Password = _messageQueueConfiguration.Password,
            Port = _messageQueueConfiguration.Port
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        // Declare exchange
        await _channel.ExchangeDeclareAsync(
            exchange: _messageQueueConfiguration.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        // Declare and bind queues
        await _channel.QueueDeclareAsync(
            queue: "incident",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await _channel.QueueBindAsync(
            queue: "incident",
            exchange: _messageQueueConfiguration.ExchangeName,
            routingKey: "fire.gis");

        await _channel.QueueBindAsync(
            queue: "incident",
            exchange: _messageQueueConfiguration.ExchangeName,
            routingKey: "fire.location");

        await _channel.QueueDeclareAsync(
            queue: "photo",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await _channel.QueueBindAsync(
            queue: "photo",
            exchange: _messageQueueConfiguration.ExchangeName,
            routingKey: "fire.photo");
    }

    private async Task<IChannel> GetChannelAsync()
    {
        await _initializationTask.Complete; // Ensure initialization is complete before accessing the channel
        
        if (_channel == null)
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");
        
        return _channel;
    }
    
    public async Task PublishToRabbitMq(string routingKey, object message, CancellationToken token)
    {
        var channel = await GetChannelAsync();
        
        var messageBody = JsonConvert.SerializeObject(message);

        var body = Encoding.UTF8.GetBytes(messageBody);

        await channel.BasicPublishAsync(
            exchange: _messageQueueConfiguration.ExchangeName,
            routingKey: routingKey,
            body: body,
            cancellationToken: token);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.DisposeAsync();
        if (_connection != null)
            await _connection.DisposeAsync();
    }
}