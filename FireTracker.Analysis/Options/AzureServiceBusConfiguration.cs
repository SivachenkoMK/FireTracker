namespace FireTracker.Analysis.Options;

public class AzureServiceBusConfiguration
{
    public string ConnectionString { get; set; } = default!;
    
    public string TopicName { get; set; } = default!;
    
    public string SubscriptionName { get; set; } = default!;
}