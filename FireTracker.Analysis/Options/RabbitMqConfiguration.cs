namespace FireTracker.Analysis.Options;

public class RabbitMqConfiguration
{
    public string HostName { get; set; } = default!;
    
    public string UserName { get; set; } = default!;
    
    public string Password { get; set; } = default!;
    
    public string InboundQueue { get; set; } = default!;
    
    public string PublishTopic { get; set; } = default!;
    
    public int Port { get; set; }
}