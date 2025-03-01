namespace FireTracker.Core.Services.Abstractions;

public interface IMessagingConsumer : IAsyncDisposable
{
    /// <summary>
    /// Fired when a message is received.
    /// </summary>
    event Func<string, string, Task>? OnMessageReceivedAsync;

    /// <summary>
    /// Starts the consumer.
    /// </summary>
    Task StartConsumingAsync(CancellationToken cancellationToken);
}