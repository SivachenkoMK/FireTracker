using System.Text;
using FireTracker.Analysis.Options;
using FireTracker.Analysis.Services.Abstractions;
using FireTracker.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FireTracker.Analysis.Services;

public class RabbitMqMessagingPublisher : IMessagingPublisher
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly AsyncLazy _initializationTask;
    private readonly RabbitMqConfiguration _rabbitMqConfiguration;

    public RabbitMqMessagingPublisher(IOptions<RabbitMqConfiguration> exchangeOptions)
    {
        _rabbitMqConfiguration = exchangeOptions.Value;
        _initializationTask = new AsyncLazy(InitializeRabbitMqAsync);
    }

    private async Task InitializeRabbitMqAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqConfiguration.HostName,
            UserName = _rabbitMqConfiguration.UserName,
            Password = _rabbitMqConfiguration.Password,
            Port = _rabbitMqConfiguration.Port
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        // Declare exchange
        await _channel.ExchangeDeclareAsync(
            exchange: _rabbitMqConfiguration.PublishTopic,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        // Declare and bind queues
        await _channel.QueueDeclareAsync(
            queue: "analysis",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await _channel.QueueBindAsync(
            queue: "analysis",
            exchange: _rabbitMqConfiguration.PublishTopic,
            routingKey: "fire.analysis");
    }

    private async Task<IChannel> GetChannelAsync()
    {
        await _initializationTask.Complete; // Ensure initialization is complete
        if (_channel == null)
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");
        return _channel;
    }

    public async Task PublishAsync(string routingKey, object message, CancellationToken cancellationToken)
    {
        var channel = await GetChannelAsync();

        var messageBody = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(messageBody);

        await channel.BasicPublishAsync(
            exchange: _rabbitMqConfiguration.PublishTopic,
            routingKey: routingKey,
            body: body,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.DisposeAsync();
        if (_connection != null)
            await _connection.DisposeAsync();
    }
}