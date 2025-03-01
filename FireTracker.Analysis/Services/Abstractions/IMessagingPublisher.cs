namespace FireTracker.Analysis.Services.Abstractions;

public interface IMessagingPublisher : IAsyncDisposable
{
    Task PublishAsync(string routingKey, object message, CancellationToken cancellationToken);
}