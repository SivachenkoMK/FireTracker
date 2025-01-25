namespace FireTracker.Analysis.Options;

// Might want to use it if passing image urls without SAS token for extra security
public class ImageStorageConfiguration
{ 
    public string ConnectionString { get; set; } = default!;
}