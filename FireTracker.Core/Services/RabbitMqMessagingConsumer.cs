using System.Text;
using FireTracker.Core.Options;
using FireTracker.Core.Services.Abstractions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FireTracker.Core.Services;

public class RabbitMqMessagingConsumer(
    ILogger<RabbitMqMessagingConsumer> logger,
    IOptions<RabbitMqConfiguration> configuration,
    IOptions<InboundQueueConfiguration> inboundQueueConfiguration)
    : IMessagingConsumer
{
    private readonly RabbitMqConfiguration _configuration = configuration.Value;
    private readonly InboundQueueConfiguration _inboundQueueConfiguration = inboundQueueConfiguration.Value;
    private IConnection? _connection;
    private IChannel? _channel;

    public event Func<string, string, Task>? OnMessageReceivedAsync;

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration.HostName,
            UserName = _configuration.UserName,
            Password = _configuration.Password,
            Port = _configuration.Port
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _inboundQueueConfiguration.IncidentQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
        
        await _channel.QueueDeclareAsync(
            queue: _inboundQueueConfiguration.AnalysisQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                logger.LogInformation("RabbitMQ Received message: {Message}", message);

                if (OnMessageReceivedAsync != null)
                {
                    await OnMessageReceivedAsync.Invoke(message, routingKey);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing RabbitMQ message");
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: cancellationToken);
            }
        };

        await Task.WhenAll(
            _channel.BasicConsumeAsync(
                queue: _inboundQueueConfiguration.IncidentQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken),
            _channel.BasicConsumeAsync(
                queue: _inboundQueueConfiguration.AnalysisQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken)
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}