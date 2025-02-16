namespace FireTracker.Api.Services.Abstractions;

public interface IMessagingService : IAsyncDisposable
{
    Task PublishAsync(string routingKey, object message, CancellationToken cancellationToken);
}