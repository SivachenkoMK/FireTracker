namespace FireTracker.Analysis.Services.Abstractions;

public interface IMessagingConsumer : IAsyncDisposable
{
    /// <summary>
    /// Fired when a message is received.
    /// </summary>
    event Func<string, Task>? OnMessageReceivedAsync;

    /// <summary>
    /// Starts the consumer.
    /// </summary>
    Task StartConsumingAsync(CancellationToken cancellationToken);
}