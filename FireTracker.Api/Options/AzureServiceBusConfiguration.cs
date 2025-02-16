namespace FireTracker.Api.Options;

public class AzureServiceBusConfiguration
{
    public string ConnectionString { get; set; } = default!;

    public string TopicName { get; set; } = default!;
}