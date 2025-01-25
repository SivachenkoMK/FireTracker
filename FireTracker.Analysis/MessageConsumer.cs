using FireTracker.Analysis.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FireTracker.Analysis;

public class MessageConsumer
{
    private readonly ILogger<MessageConsumer> _logger;
    private readonly MessageQueueConfiguration _configuration;

    public event Func<string, Task>? OnMessageReceivedAsync;

    public MessageConsumer(ILogger<MessageConsumer> logger, IOptions<MessageQueueConfiguration> configuration)
    {
        _logger = logger;
        _configuration = configuration.Value;
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration.HostName,
            UserName = _configuration.UserName,
            Password = _configuration.Password,
            Port = _configuration.Port
        };

        var connection = await factory.CreateConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        
        await channel.QueueDeclareAsync(queue: _configuration.InboundQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message: {Message}", message);

                if (OnMessageReceivedAsync != null)
                {
                    await OnMessageReceivedAsync.Invoke(message);
                }

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(queue: _configuration.InboundQueue,
            autoAck: false,
            consumer: consumer, cancellationToken: cancellationToken);
    }
}