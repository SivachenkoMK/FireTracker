namespace FireTracker.Api.Options;

public class ImageStorageConfiguration
{
    public string ContainerName { get; set; } = default!;
    
    public string ConnectionString { get; set; } = default!;
}