namespace FireTracker.Api.Options;

public class RabbitMqConfiguration
{
    public string HostName { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ExchangeName { get; set; } = default!;
    public int Port { get; set; }
}